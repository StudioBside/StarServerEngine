﻿namespace CutEditor.ViewModel;

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using CutEditor.ViewModel.Detail;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmHome : VmPageBase
{
    private const string ExportRoot = "./Export";
    private const string DefaultFilter = "[All]";
    private readonly List<CutScene> cutScenes = new();
    private readonly HashSet<string> filters = new();
    private readonly IServiceProvider services;
    private readonly ISearchableCollection<CutScene> filteredList;
    private CutScene? selectedCutScene;
    private string searchKeyword = string.Empty;
    private string selectedFilter = DefaultFilter;

    public VmHome(IServiceProvider services)
    {
        this.Title = "컷신 목록";
        this.services = services;
        this.filteredList = services.GetRequiredService<ISearchableCollectionProvider>().Build(this.cutScenes);
        this.ReadPickedCommand = new RelayCommand<CutScene>(this.OnReadPicked);
        this.NewFileCommand = new AsyncRelayCommand(this.OnNewFile);
        this.ExportCommand = new RelayCommand<CutScene>(this.OnExport);
        this.ImportCommand = new RelayCommand<CutScene>(this.OnImport);
        this.OpenExportRootCommand = new RelayCommand(this.OnOpenExportRoot);

        this.filters.Add(DefaultFilter);
        this.filteredList.SetSubFilter(e =>
        {
            return this.selectedFilter == DefaultFilter || e.CutsceneFilter == this.selectedFilter;
        });
    }

    public CutScene? SelectedCutScene
    {
        get => this.selectedCutScene;
        set => this.SetProperty(ref this.selectedCutScene, value);
    }

    public IEnumerable FilteredFiles => this.filteredList.List;
    public IEnumerable<string> Filters => this.filters;
    public int FilteredCount => this.filteredList.FilteredCount;
    public int TotalCount => this.filteredList.SourceCount;

    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    public string SelectedFilter
    {
        get => this.selectedFilter;
        set => this.SetProperty(ref this.selectedFilter, value);
    }

    public ICommand ReadPickedCommand { get; }
    public ICommand NewFileCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand OpenExportRootCommand { get; }

    public void AddCutScenes(IEnumerable<CutScene> cutScenes)
    {
        this.cutScenes.Clear();
        this.cutScenes.AddRange(cutScenes);

        foreach (var filter in cutScenes.Select(e => e.CutsceneFilter).Distinct())
        {
            this.filters.Add(filter);
        }
                
        this.filteredList.Refresh();
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SearchKeyword):
            case nameof(this.SelectedFilter):
                if (this.selectedCutScene is not null)
                {
                    this.SelectedCutScene = null;
                }

                this.filteredList.Refresh(this.searchKeyword);
                this.OnPropertyChanged(nameof(this.FilteredCount));
                break;
        }
    }

    private async Task OnNewFile()
    {
        var userinputprovider = this.services.GetRequiredService<IUserInputProvider<string>>();
        var fileName = await userinputprovider.PromptAsync("새로운 컷신을 만듭니다", "컷신 이름을 입력하세요");

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        this.services.GetRequiredService<IPageRouter>()
            .Route(new VmCuts.CreateParam(fileName, CutUid: 0));
    }

    private void OnReadPicked(CutScene? scene)
    {
        if (scene is null)
        {
            Log.Error($"argument is null");
            return;
        }

        this.services.GetRequiredService<IPageRouter>()
            .Route(new VmCutsSummary.CreateParam(scene.FileName));
    }

    private void OnExport(CutScene? scene)
    {
        if (scene is null)
        {
            Log.Error($"argument is null");
            return;
        }

        var current = ServiceTime.Recent;
        var fileName = $"{ExportRoot}/{scene.FileName}_{current.ToFileString()}.xlsx";
        var nameOnly = Path.GetFileNameWithoutExtension(fileName);

        var cuts = CutFileIo.LoadCutData(scene.FileName);
        if (cuts.Any() == false)
        {
            Log.Error($"파일 로딩에 실패했습니다. fileName:{CutFileIo.GetTextFileName(scene.FileName)}");
            return;
        }

        var uidGenerator = new CutUidGenerator(cuts);
        foreach (var cut in cuts.Where(e => e.Choices.Any()))
        {
            var choiceUidGenerator = new ChoiceUidGenerator(cut.Uid, cut.Choices);
        }

        var writer = this.services.GetRequiredService<IExcelFileWriter>();
        if (writer.Write(fileName, cuts.SelectMany(e => e.ToOutputExcelType())) == false)
        {
            Log.Error($"파일 생성에 실패했습니다. fileName:{nameOnly}");
            return;
        }

        Log.Info($"파일 생성 완료. fileName:{nameOnly}");
    }

    private void OnOpenExportRoot()
    {
        var fullPath = Path.GetFullPath(ExportRoot);
        Process.Start("explorer.exe", fullPath);
    }

    private void OnImport(CutScene? scene)
    {
        var picker = this.services.GetRequiredService<IFilePicker>();
        var fileName = picker.PickFile(Environment.CurrentDirectory, "엑셀 파일 (*.xlsx)|*.xlsx");
    }
}