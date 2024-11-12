namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Dialogs;
using CutEditor.Model.Interfaces;
using Shared.Templet.Strings;
using Wpf.Ui;
using Wpf.Ui.Controls;

using ResultType = CutEditor.Model.Interfaces.IModelEditor<System.Collections.Generic.IList<Shared.Templet.Strings.StringElement>>.PickResult;

internal sealed class UnitNameEditor(IContentDialogService contentDialogService)
    : IModelEditor<IList<StringElement>>
{
    public async Task<ResultType> EditAsync(IList<StringElement>? current)
    {
        if (current is null)
        {
            return new ResultType(Data: null, IsCanceled: true);
        }

        var dialog = new UnitNameEditorDialog(contentDialogService.GetDialogHost(), current);
        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.Primary => new ResultType(dialog.SelectedList, IsCanceled: false),
            ContentDialogResult.Secondary => new ResultType(Data: null, IsCanceled: false),
            _ => new ResultType(Data: null, IsCanceled: true),
        };
    }
}
