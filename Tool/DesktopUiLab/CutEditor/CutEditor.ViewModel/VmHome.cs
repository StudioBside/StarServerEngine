namespace CutEditor.ViewModel;

using System.Collections;
using System.ComponentModel;
using CutEditor.Model;
using Du.Core.Bases;
using Du.Core.Interfaces;

public sealed class VmHome : VmPageBase
{
    private readonly List<CutScene> cutScenes = new();
    private CutScene? selectedCutScene;
    private IFilteredCollection filteredList;
    private string searchKeyword = string.Empty;

    public VmHome(IFilteredCollectionProvider collectionViewProvider)
    {
        this.Title = "컷신 목록";
        this.filteredList = collectionViewProvider.Build(this.cutScenes);
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
        }
    }
}