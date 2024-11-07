namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Shared.Templet.TempletTypes;
using Wpf.Ui;

internal sealed class UnitPicker(IContentDialogService contentDialogService)
    : ITempletPicker<Unit>
{
    public async Task<ITempletPicker<Unit>.PickResult> Pick()
    {
        var dialog = new UnitPickerDialog(contentDialogService.GetDialogHost());
        var result = await dialog.ShowAsync();
        return result switch
        {
            Wpf.Ui.Controls.ContentDialogResult.Primary => new ITempletPicker<Unit>.PickResult(dialog.SelectedUnit, false),
            Wpf.Ui.Controls.ContentDialogResult.Secondary => new ITempletPicker<Unit>.PickResult(null, false),
            _ => new ITempletPicker<Unit>.PickResult(null, true),
        };
    }
}
