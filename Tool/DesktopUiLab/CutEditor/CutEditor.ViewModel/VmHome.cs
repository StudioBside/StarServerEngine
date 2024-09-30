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

public sealed class VmHome : VmPageBase
{
    private readonly List<CutScene> cutScenes = new();
    private readonly IFilteredCollection filteredList;
    private CutScene? selectedCutScene;
    private string searchKeyword = string.Empty;

    public VmHome(IFilteredCollectionProvider collectionViewProvider)
    {
        this.Title = "컷신 목록";
        this.filteredList = collectionViewProvider.Build(this.cutScenes);
        this.StartEditCommand = new RelayCommand(this.OnStartEdit, () => this.selectedCutScene is not null);
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

    public IRelayCommand StartEditCommand { get; }

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
                this.filteredList.Refresh(this.searchKeyword);
                break;

            case nameof(this.SelectedCutScene):
                this.StartEditCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void OnStartEdit()
    {
        if (this.selectedCutScene is null)
        {
            Log.Error($"{nameof(this.OnStartEdit)}: SelectedCutScene is null");
            return;
        }
   
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgExtract.xaml"));
    }
}