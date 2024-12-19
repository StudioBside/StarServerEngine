namespace CutEditor.ViewModel;

using System.Collections;
using System.ComponentModel;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Shared.Templet.TempletTypes;

public sealed class VmSkills : VmPageBase
{
    private readonly ISearchableCollection<SkillTemplet> filteredList;
    private string searchKeyword = string.Empty;

    public VmSkills(ISearchableCollectionProvider provider)
    {
        this.Title = "스킬 리스트";
        this.filteredList = provider.Build(SkillTemplet.Values.OrderBy(e => e.Id));
    }

    public IEnumerable FilteredList => this.filteredList.List;
    public int FilteredCount => this.filteredList.FilteredCount;
    public int TotalCount => this.filteredList.SourceCount;
    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SearchKeyword):

                this.filteredList.Refresh(this.searchKeyword);
                this.OnPropertyChanged(nameof(this.FilteredCount));
                break;
        }
    }
}
