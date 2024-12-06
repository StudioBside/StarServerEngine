namespace CutEditor.ViewModel.UndoCommands;

using Du.Core.Interfaces;
using NKM;

internal sealed class SetUnitPos(VmCut vmCut, CutsceneUnitPos posAfter) : IDormammu
{
    private readonly CutsceneUnitPos posBefore = vmCut.Cut.UnitPos;

    public static SetUnitPos? Create(VmCut vmCut, CutsceneUnitPos posAfter)
    {
        if (vmCut.Cut.UnitPos == posAfter)
        {
            return null;
        }

        return new SetUnitPos(vmCut, posAfter);
    }

    public void Redo()
    {
        vmCut.Cut.UnitPos = posAfter;
    }

    public void Undo()
    {
        vmCut.Cut.UnitPos = this.posBefore;
    }
}
