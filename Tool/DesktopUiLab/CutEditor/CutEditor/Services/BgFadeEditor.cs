namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Controls;

internal sealed class BgFadeEditor(IContentDialogService contentDialogService)
    : IModelEditor<BgFadeInOut>
{
    public async Task<IModelEditor<BgFadeInOut>.PickResult> EditAsync(BgFadeInOut? current)
    {
        var dialog = new BgFadeEditorDialog(contentDialogService.GetDialogHost(), current);
        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.Primary => new IModelEditor<BgFadeInOut>.PickResult(dialog.Data, false),
            ContentDialogResult.Secondary => new IModelEditor<BgFadeInOut>.PickResult(null, false),
            _ => new IModelEditor<BgFadeInOut>.PickResult(null, true),
        };
    }
}
