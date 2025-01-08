namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Detail;
using CutEditor.Model.Interfaces;
using Wpf.Ui;
using DialogResult = Wpf.Ui.Controls.ContentDialogResult;

internal sealed class UnitVariantPicker(IContentDialogService contentDialogService)
    : IGeneralPicker<UnitVariant>
{
    private UnitVariant? currentValue;

    void IGeneralPicker<UnitVariant>.SetCurrentValues(UnitVariant? currentValue)
    {
        this.currentValue = currentValue;
    }

    public async Task<IGeneralPicker<UnitVariant>.PickResult> Pick()
    {
        var dialog = new UnitPickerDialog(contentDialogService.GetDialogHost(), enableIdConst: true);
        if (this.currentValue is not null)
        {
            dialog.SetCurrentValue(this.currentValue);
        }

        var result = await dialog.ShowAsync();
        return result switch
        {
            DialogResult.Primary => new IGeneralPicker<UnitVariant>.PickResult(dialog.ResultVariant, false),
            DialogResult.Secondary => new IGeneralPicker<UnitVariant>.PickResult(null, false),
            _ => new IGeneralPicker<UnitVariant>.PickResult(null, true),
        };
    }
}
