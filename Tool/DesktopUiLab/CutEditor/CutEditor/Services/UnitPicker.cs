namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Shared.Templet.TempletTypes;
using Wpf.Ui;

internal sealed class UnitPicker(UnitPickerDialog dialog, IContentDialogService contentDialogService)
    : IUnitPicker
{
    public async Task<Unit?> PickUnit()
    {
        dialog.DialogHost = contentDialogService.GetDialogHost();
        _ = await dialog.ShowAsync();
        return dialog.SelectedUnit;
    }
}
