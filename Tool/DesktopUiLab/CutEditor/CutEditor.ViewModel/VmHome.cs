namespace CutEditor.ViewModel;

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Logging;
using CutEditor.Model;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Du.Core.Models;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmHome : VmPageBase
{
    private readonly List<CutScene> cutScenes = new();
    private readonly IServiceProvider services;
    private readonly IFilteredCollection filteredList;
    private CutScene? selectedCutScene;
    private string searchKeyword = string.Empty;

    public VmHome(IServiceProvider services)
    {
        this.Title = "컷신 목록";
        this.services = services;
        this.filteredList = services.GetRequiredService<IFilteredCollectionProvider>().Build(this.cutScenes);
        this.EditSelectedCommand = new RelayCommand(this.OnEditSelected, () => this.selectedCutScene is not null);
        this.EditPickedCommand = new RelayCommand<CutScene>(this.OnEditPicked);
        this.NewFileCommand = new AsyncRelayCommand(this.OnNewFile);
    }

    public IList<CutScene> CutScenes => this.cutScenes;
    public CutScene? SelectedCutScene
    {
        get => this.selectedCutScene;
        set => this.SetProperty(ref this.selectedCutScene, value);
    }

    public IEnumerable FilteredFiles => this.filteredList.List;

    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    public IRelayCommand EditSelectedCommand { get; }
    public IRelayCommand EditPickedCommand { get; }
    public ICommand NewFileCommand { get; }

    public void AddCutScenes(IEnumerable<CutScene> cutScenes)
    {
        this.cutScenes.Clear();
        this.cutScenes.AddRange(cutScenes);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SearchKeyword):
                if (this.selectedCutScene is not null)
                {
                    this.SelectedCutScene = null;
                }

                this.filteredList.Refresh(this.searchKeyword);
                break;

            case nameof(this.SelectedCutScene):
                this.EditSelectedCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void OnEditSelected()
    {
        if (this.selectedCutScene is null)
        {
            Log.Error($"{nameof(this.OnEditSelected)}: SelectedCutScene is null");
            return;
        }
   
        VmGlobalState.Instance.ReserveVmCuts(new VmCuts.CrateParam { CutScene = this.selectedCutScene });
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgCuts.xaml"));
    }

    private void OnEditPicked(CutScene? scene)
    {
        if (scene is null)
        {
            Log.Error($"argument is null");
            return;
        }

        VmGlobalState.Instance.ReserveVmCuts(new VmCuts.CrateParam { CutScene = scene });
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgCuts.xaml"));
    }

    private async Task OnNewFile()
    {
        var userinputprovider = this.services.GetRequiredService<IUserInputProvider<string>>();
        var fileName = await userinputprovider.PromptAsync("새로운 컷신을 만듭니다", "컷신 이름을 입력하세요");

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        VmGlobalState.Instance.ReserveVmCuts(new VmCuts.CrateParam { NewFileName = fileName });
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgCuts.xaml"));
    }
}