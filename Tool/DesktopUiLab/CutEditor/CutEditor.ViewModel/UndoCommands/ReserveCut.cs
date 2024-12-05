namespace CutEditor.ViewModel.UndoCommands;

internal sealed class ReserveCut : DeleteCuts
{
    private ReserveCut(VmCuts vmCuts, IReadOnlyList<VmCut> targets) : base(vmCuts, targets)
    {
    }

    public static new ReserveCut? Create(VmCuts vmCuts)
    {
        if (vmCuts.SelectedCuts.Count == 0)
        {
            return null;
        }

        return new ReserveCut(vmCuts, vmCuts.SelectedCuts.ToArray());
    }

    public override void Redo()
    {
        base.Redo();
        this.VmCuts.CutPaster.SetReserved(this.Targets);
    }

    public override void Undo()
    {
        base.Undo();
        this.VmCuts.CutPaster.ClearReserved();
    }
}
