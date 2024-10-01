namespace CutEditor.ViewModel;

using System.ComponentModel;
using CutEditor.Model;
using Du.Core.Bases;

public sealed class VmCuts : VmPageBase
{
    private static CutScene lastCutSceneHistory = null!;

    private readonly CutScene cutscene;

    public VmCuts(VmHome vmHome)
    {
        this.cutscene = vmHome.SelectedCutScene ?? lastCutSceneHistory;
        this.Title = $"{this.cutscene.Title} - {this.cutscene.FileName}";

        lastCutSceneHistory = this.cutscene;
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        //switch (e.PropertyName)
        //{
        //    case nameof(this.SearchKeyword):
        //        this.filteredList.Refresh(this.searchKeyword);
        //        break;

        //    case nameof(this.SelectedCutScene):
        //        this.StartEditCommand.NotifyCanExecuteChanged();
        //        break;
        //}
    }
}