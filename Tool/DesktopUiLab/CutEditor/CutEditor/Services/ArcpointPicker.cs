namespace CutEditor.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Shared.Templet.TempletTypes;
using Wpf.Ui;
using Wpf.Ui.Controls;

internal sealed class ArcpointPicker(IContentDialogService contentDialogService)
    : IGeneralPicker<LobbyItem>
{
    public async Task<IGeneralPicker<LobbyItem>.PickResult> Pick()
    {
        var dialog = new ArcpointPickerDialog(contentDialogService.GetDialogHost());
        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.Primary => new IGeneralPicker<LobbyItem>.PickResult(dialog.Selected, false),
            ContentDialogResult.Secondary => new IGeneralPicker<LobbyItem>.PickResult(null, false),
            _ => new IGeneralPicker<LobbyItem>.PickResult(null, true),
        };
    }

    public Task<IGeneralPicker<LobbyItem>.PickResult> Pick(IEnumerable<LobbyItem> candidates)
    {
        return this.Pick();
    }
}
