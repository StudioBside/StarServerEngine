namespace CutEditor.Services;

using System.Threading.Tasks;
using Cs.Logging;
using CutEditor.Dialogs;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Wpf.Ui;

using DlgResult = Wpf.Ui.Controls.ContentDialogResult;

internal sealed class UnitReplaceQuery(IContentDialogService contentDialogService)
    : IUnitReplaceQuery
{
    public async Task<IUnitReplaceQuery.Result> QueryAsync(IEnumerable<Cut> cuts)
    {
        var cancelResult = new IUnitReplaceQuery.Result(null!, null!, IsCanceled: true);

        var units = cuts
            .Where(e => e.Unit is not null)
            .Select(e => e.Unit!)
            .Distinct()
            .ToArray();

        if (units.Length == 0)
        {
            Log.Info("유닛을 지정한 컷이 존재하지 않습니다. 일괄 변경이 불가합니다.");
            return cancelResult;
        }

        var dialog = new UnitReplaceDialog(contentDialogService.GetDialogHost(), units);
        var result = await dialog.ShowAsync();
        if (result != DlgResult.Primary)
        {
            return cancelResult;
        }

        return new IUnitReplaceQuery.Result(dialog.PrevUnit, dialog.AfterUnit, IsCanceled: false);
    }
}
