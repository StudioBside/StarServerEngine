namespace CutEditor.ViewModel;

using System.ComponentModel;
using Du.Core.Bases;
using Shared.Templet.UnitScripts;

public sealed class VmUnitScriptDetail : VmPageBase
{
    public VmUnitScriptDetail(UnitScript script)
    {
        this.Script = script;
        this.Title = script.FileName;
    }

    public UnitScript Script { get; }

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
