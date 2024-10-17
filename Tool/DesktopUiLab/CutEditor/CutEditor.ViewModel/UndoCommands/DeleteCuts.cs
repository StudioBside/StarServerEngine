namespace CutEditor.ViewModel.UndoCommands;

using System;
using Du.Core.Interfaces;

internal sealed class DeleteCuts(VmCuts vmCuts) : IDormammu
{
    private readonly IReadOnlyList<VmCut> cuts = vmCuts.Cuts.ToArray();
    private readonly IReadOnlyList<VmCut> selected = vmCuts.SelectedCuts.ToArray();

    public static DeleteCuts? Create(VmCuts vmCuts)
    {
        if (vmCuts.SelectedCuts.Count == 0)
        {
            return null;
        }

        return new DeleteCuts(vmCuts);
    }

    public void Redo()
    {
        foreach (var cut in this.selected)
        {
            vmCuts.Cuts.Remove(cut);
        }

        vmCuts.SelectedCuts.Clear();
    }

    public void Undo()
    {
        // Undo를 위해 삭제된 항목을 다시 추가
        int index = 0;
        foreach (var cut in this.cuts)
        {
            if (index < vmCuts.Cuts.Count && vmCuts.Cuts[index] == cut)
            {
                index++;
                continue;
            }

            vmCuts.Cuts.Insert(index, cut);
            index++;
        }

        // 선택된 항목을 다시 선택
        vmCuts.SelectedCuts.Clear();
        foreach (var cut in this.selected)
        {
            vmCuts.SelectedCuts.Add(cut);
        }
    }
}
