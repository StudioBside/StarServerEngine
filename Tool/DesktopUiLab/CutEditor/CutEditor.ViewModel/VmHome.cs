namespace CutEditor.ViewModel;

using System.Collections;
using System.ComponentModel;
using CutEditor.Model;
using Du.Core.Bases;
using Du.Core.Interfaces;

public sealed class VmHome(IFilteredCollectionProvider collectionViewProvider) : VmPageBase
{
    private readonly List<CutScene> cutScenes = new();
    private CutScene? selectedCutScene;
    private IEnumerable filteredList = null!;
    private string searchKeyword = string.Empty;

    public IList<CutScene> CutScenes => this.cutScenes;
    public CutScene? SelectedCutScene
    {
        get => this.selectedCutScene;
        set => this.SetProperty(ref this.selectedCutScene, value);
    }

    public IEnumerable FilteredList
    {
        get => this.filteredList;
        set => this.SetProperty(ref this.filteredList, value);
    }

    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

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
                this.FilterFiles();
                break;
        }
    }

    private void FilterFiles()
    {
        if (string.IsNullOrEmpty(this.searchKeyword))
        {
            this.filteredList = this.cutScenes;
        }
        else
        {
            this.filteredList = collectionViewProvider.Build(
                this.cutScenes,
                e => e.IsTarget(this.searchKeyword));
        }
    }
}