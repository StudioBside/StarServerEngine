namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Shared.Templet.TempletTypes;
using Wpf.Ui;

internal sealed class UnitPopup(IContentDialogService contentDialogService)
    : IUnitPopup
{
    Task IUnitPopup.Show(Unit unitTemplet)
    {
        var dialog = new UnitPopupDialog(contentDialogService.GetDialogHost(), unitTemplet);
        return dialog.ShowAsync();
    }
}
