namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Wpf.Ui;

internal sealed class BgFilePicker(IContentDialogService contentDialogService)
    : IAssetPicker
{
    public async Task<IAssetPicker.PickResult> PickAsset()
    {
        var dialog = new BgFilePickerDialog(
            "배경 파일 선택",
            contentDialogService.GetDialogHost());
        var result = await dialog.ShowAsync();
        return result switch
        {
            Wpf.Ui.Controls.ContentDialogResult.Primary => new IAssetPicker.PickResult(dialog.SelectedFileName, false),
            Wpf.Ui.Controls.ContentDialogResult.Secondary => new IAssetPicker.PickResult(null, false),
            _ => new IAssetPicker.PickResult(null, true),
        };
    }
}
