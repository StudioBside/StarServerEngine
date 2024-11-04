namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Wpf.Ui;

internal sealed class ArcpointPicker(IContentDialogService contentDialogService)
    : IArcpointPicker
{
    public async Task<IArcpointPicker.PickResult> Pick()
    {
        var dialog = new ArcpointPickerDialog(contentDialogService.GetDialogHost());
        var result = await dialog.ShowAsync();
        return result switch
        {
            Wpf.Ui.Controls.ContentDialogResult.Primary => new IArcpointPicker.PickResult(dialog.Selected, false),
            Wpf.Ui.Controls.ContentDialogResult.Secondary => new IArcpointPicker.PickResult(null, false),
            _ => new IArcpointPicker.PickResult(null, true),
        };
    }
}
