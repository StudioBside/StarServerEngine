namespace CutEditor.ViewModel;

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Detail;
using CutEditor.Model.ExcelFormats;
using CutEditor.Model.Interfaces;
using CutEditor.ViewModel.Detail;
using CutEditor.ViewModel.L10n;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmHome : VmPageBase
{
    private const string DefaultFilter = "[All]";
    private static string searchKeyword = string.Empty;
    private static string selectedFilter = DefaultFilter;

    private readonly HashSet<string> filters = new();
    private readonly IServiceProvider services;
    private readonly ISearchableCollection<CutScene> filteredList;
    private CutScene? selectedCutScene;

    public VmHome(IServiceProvider services)
    {
        this.Title = "컷신 목록";
        this.services = services;
        this.filteredList = services.GetRequiredService<ISearchableCollectionProvider>()
            .Build(CutSceneContainer.Instance.CutScenes);
        this.ReadPickedCommand = new RelayCommand<CutScene>(this.OnReadPicked);
        this.NewFileCommand = new AsyncRelayCommand(this.OnNewFile);
        this.ExportCommand = new RelayCommand<CutScene>(this.OnExport);
        this.EditShortenCommand = new AsyncRelayCommand<CutScene>(this.OnEditShorten);
        this.ExportShortenCommand = new AsyncRelayCommand(this.OnExportShorten);
        this.ApplySearchKeywordCommand = new RelayCommand(this.OnApplySearchKeyword);

        this.filters.Add(DefaultFilter);
        foreach (var filter in CutSceneContainer.Instance.CutScenes.Select(e => e.CutsceneFilter).Distinct())
        {
            this.filters.Add(filter);
        }

        this.filteredList.SetSubFilter(e =>
        {
            return selectedFilter == DefaultFilter || e.CutsceneFilter == selectedFilter;
        });

        this.filteredList.Refresh(searchKeyword);
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
        get => searchKeyword;
        set => this.SetProperty(ref searchKeyword, value);
    }

    public string SelectedFilter
    {
        get => selectedFilter;
        set => this.SetProperty(ref selectedFilter, value);
    }

    public ICommand ReadPickedCommand { get; }
    public ICommand NewFileCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand EditShortenCommand { get; }
    public ICommand ExportShortenCommand { get; }
    public ICommand ApplySearchKeywordCommand { get; }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SearchKeyword):
            case nameof(this.SelectedFilter):
                // 입력마다 필터가 갱신되면서 버벅이는 현상의 개선 요청이 있어, 별개 트리거(엔터키입력)로 변경합니다.
                //if (this.selectedCutScene is not null)
                //{
                //    this.SelectedCutScene = null;
                //}

                //this.filteredList.Refresh(searchKeyword);
                //this.OnPropertyChanged(nameof(this.FilteredCount));
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

        var fileName = $"{VmGlobalState.ExportRoot}/{scene.FileName}.xlsx";
        var nameOnly = Path.GetFileNameWithoutExtension(fileName);

        // 동일한 이름의 파일이 존재한다면 삭제.
        if (FileSystem.SafeDelete(fileName) == false)
        {
            return;
        }

        var cuts = CutFileIo.LoadCutData(scene.FileName, isShorten: false);
        if (cuts.Any() == false)
        {
            Log.Error($"파일 로딩에 실패했습니다. fileName:{CutFileIo.GetTextFileName(scene.FileName, isShorten: false)}");
            return;
        }

        using var writer = this.services.GetRequiredService<IExcelFileWriter>();
        if (writer.Write(fileName, cuts.SelectMany(e => e.ToOutputExcelType())) == false)
        {
            Log.Error($"파일 생성에 실패했습니다. fileName:{nameOnly}");
            return;
        }

        Log.Info($"파일 생성 완료. fileName:{nameOnly}");
    }

    private async Task OnExportShorten()
    {
        var notifier = this.services.GetRequiredService<IUserWaitingNotifier>();
        using var closer = await notifier.StartWait("단축 컷신 파일을 생성 중입니다...");
        await Task.Run(() =>
        {
            var outputFileName = Path.Combine(VmGlobalState.ExportRoot, VmGlobalState.ShortenExportFileName);
            using var writer = this.services.GetRequiredService<IExcelFileWriter>();
            if (writer.CreateSheet<ShortenCutOutputExcelFormat>(outputFileName) == false)
            {
                Log.Error($"파일 생성에 실패했습니다. fileName:{VmGlobalState.ShortenExportFileName}");
                return;
            }

            foreach (var (fileName, cuts) in ShortenCuts.LoadAll())
            {
                if (writer.AppendToSheet(cuts.SelectMany(e => e.ToShortenOutputExcelType(fileName))) == false)
                {
                    Log.Error($"엑셀 시트에 데이터를 적을 수 없습니다. 컷신이름:{fileName}");
                    return;
                }
            }

            if (writer.CloseSheet<ShortenCutOutputExcelFormat>() == false)
            {
                Log.Error($"엑셀 시트를 닫을 수 없습니다. fileName:{VmGlobalState.ShortenExportFileName}");
                return;
            }

            Log.Info($"파일 생성 완료. fileName:{VmGlobalState.ShortenExportFileName}");
        });
    }

    private async Task OnEditShorten(CutScene? scene)
    {
        if (scene is null)
        {
            Log.Error($"argument is null");
            return;
        }

        var fileName = CutFileIo.GetTextFileName(scene.FileName, isShorten: true);
        if (File.Exists(fileName) == false)
        {
            var prompt = this.services.GetRequiredService<IUserInputProvider<bool>>();
            var result = await prompt.PromptAsync($"[{scene.Title}]는 단축 컷신이 없습니다", "새 파일을 생성하시겠습니까?");
            if (result == false)
            {
                return;
            }
        }

        var router = this.services.GetRequiredService<IPageRouter>();
        router.Route(new VmCuts.CreateParam(scene.FileName, CutUid: 0)
        {
            IsShorten = true,
        });
    }

    private void OnApplySearchKeyword()
    {
        if (this.selectedCutScene is not null)
        {
            this.SelectedCutScene = null;
        }

        this.filteredList.Refresh(searchKeyword);
        this.OnPropertyChanged(nameof(this.FilteredCount));
    }
}