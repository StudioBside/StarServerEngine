namespace CutEditor.ViewModel;

using System.Collections;
using System.ComponentModel;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.TempletTypes;

public sealed class VmBuffs : VmPageBase
{
    private readonly ISearchableCollection<BuffTemplet> filteredList;
    private string searchKeyword = string.Empty;

    public VmBuffs(ISearchableCollectionProvider provider)
    {
        this.Title = "버프 리스트";
        this.filteredList = provider.Build(BuffTemplet.Values.OrderBy(e => e.Id));
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
