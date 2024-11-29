namespace CutEditor.ViewModel;

using System.ComponentModel;
using Du.Core.Bases;
using Shared.Templet.TempletTypes;

public sealed class VmUnitDetail : VmPageBase
{
    public VmUnitDetail(Unit templet)
    {
        this.Templet = templet;
        this.Title = templet.DebugName;
    }

    public Unit Templet { get; }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        //switch (e.PropertyName)
        //{
        //    case nameof(this.SearchKeyword):
        //        this.filteredList.Refresh(this.searchKeyword);
        //        this.OnPropertyChanged(nameof(this.FilteredCount));
        //        break;
        //}
    }
}
