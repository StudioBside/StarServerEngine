namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Shared.Templet.TempletTypes;
using Wpf.Ui;
using DialogResult = Wpf.Ui.Controls.ContentDialogResult;

internal sealed class UnitPicker(IContentDialogService contentDialogService)
    : IGeneralPicker<Unit>
{
    public async Task<IGeneralPicker<Unit>.PickResult> Pick()
    {
        var dialog = new UnitPickerDialog(contentDialogService.GetDialogHost(), enableIdConst: false);
        var result = await dialog.ShowAsync();
        return result switch
        {
            DialogResult.Primary => new IGeneralPicker<Unit>.PickResult(dialog.ResultUnit, false),
            DialogResult.Secondary => new IGeneralPicker<Unit>.PickResult(null, false),
            _ => new IGeneralPicker<Unit>.PickResult(null, true),
        };
    }
}
