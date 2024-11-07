namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Shared.Templet.TempletTypes;
using Wpf.Ui;
using Wpf.Ui.Controls;

internal sealed class ArcpointPicker(IContentDialogService contentDialogService)
    : ITempletPicker<LobbyItem>
{
    public async Task<ITempletPicker<LobbyItem>.PickResult> Pick()
    {
        var dialog = new ArcpointPickerDialog(contentDialogService.GetDialogHost());
        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.Primary => new ITempletPicker<LobbyItem>.PickResult(dialog.Selected, false),
            ContentDialogResult.Secondary => new ITempletPicker<LobbyItem>.PickResult(null, false),
            _ => new ITempletPicker<LobbyItem>.PickResult(null, true),
        };
    }
}
