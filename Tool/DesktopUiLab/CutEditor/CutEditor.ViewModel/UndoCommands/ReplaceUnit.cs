namespace CutEditor.ViewModel.UndoCommands;

using Cs.Logging;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Shared.Templet.TempletTypes;

internal sealed class ReplaceUnit : IDormammu
{
    private readonly IReadOnlyList<VmCut> targetCuts;
    private readonly Unit prevUnit;
    private readonly Unit afterUnit;

    private ReplaceUnit(VmCuts vmCuts, IUnitReplaceQuery.Result queryResult, IReadOnlyList<VmCut> targetCuts)
    {
        this.targetCuts = targetCuts;
        this.prevUnit = queryResult.PrevUnit;
        this.afterUnit = queryResult.AfterUnit;
    }

    public static ReplaceUnit? Create(VmCuts vmCuts, IUnitReplaceQuery.Result queryResult)
    {
        if (queryResult.IsCanceled)
        {
            return null;
        }

        var targetCuts = vmCuts.Cuts.Where(e => e.Cut.Unit == queryResult.PrevUnit).ToArray();
        if (targetCuts.Length == 0)
        {
            return null;
        }

        return new ReplaceUnit(vmCuts, queryResult, targetCuts);
    }

    public void Redo()
    {
        foreach (var vmCut in this.targetCuts)
        {
            vmCut.Cut.Unit = this.afterUnit;
        }

        Log.Info($"{this.targetCuts.Count} 건의 데이터에서 유닛 변경: {this.prevUnit} -> {this.afterUnit}");
    }

    public void Undo()
    {
        foreach (var vmCut in this.targetCuts)
        {
            vmCut.Cut.Unit = this.prevUnit;
        }

        Log.Info($"{this.targetCuts.Count} 건의 데이터에서 유닛 변경: {this.afterUnit} -> {this.prevUnit}");
    }
}
