namespace CutEditor.ViewModel.UndoCommands;

using System;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.ViewModel.UndoCommands.PasteCut;

internal sealed class PasteCut(
    VmCuts vmCuts,
    IReadOnlyList<VmCut> targets,
    int positionIndex,
    PasteDirection direction,
    bool reReserveWhenUndo) : IDormammu
{
    public enum PasteDirection
    {
        Upside,
        Downside,
    }

    public static PasteCut? Create(VmCuts vmCuts, PasteDirection direction, bool reReserve)
    {
        if (vmCuts.CutPaster.Reserved.Count == 0)
        {
            return null;
        }

        int positionIndex = vmCuts.Cuts.Count - 1; // 선택컷이 없으면 마지막에 붙인다.
        if (vmCuts.SelectedCuts.Count > 0)
        {
            positionIndex = vmCuts.Cuts.IndexOf(vmCuts.SelectedCuts[0]);
        }

        return new PasteCut(vmCuts, vmCuts.CutPaster.Reserved.ToArray(), positionIndex, direction, reReserve);
    }

    public void Redo()
    {
        int index = direction switch
        {
            PasteDirection.Upside => positionIndex,
            PasteDirection.Downside => positionIndex + 1,
            _ => throw new InvalidOperationException(),
        };

        foreach (var cut in targets)
        {
            vmCuts.Cuts.Insert(index, cut);
            index++;
        }

        vmCuts.SelectedCuts.Clear();
        foreach (var cut in targets)
        {
            vmCuts.SelectedCuts.Add(cut);
        }

        var controller = vmCuts.Services.GetRequiredService<ICutsListController>();
        controller.FocusElement(index);

        vmCuts.CutPaster.ClearReserved();
    }

    public void Undo()
    {
        // 삭제된 후 선택될 항목을 결정.
        int selectedIndex = vmCuts.Cuts.IndexOf(targets[0]);
        if (selectedIndex > 0)
        {
            selectedIndex--;
        }

        foreach (var cut in targets)
        {
            vmCuts.Cuts.Remove(cut);
        }

        var selected = vmCuts.Cuts[selectedIndex];
        vmCuts.SelectedCuts.Clear();
        vmCuts.SelectedCuts.Add(vmCuts.Cuts[selectedIndex]);

        var controller = vmCuts.Services.GetRequiredService<ICutsListController>();
        controller.FocusElement(selectedIndex);

        if (reReserveWhenUndo)
        {
            // 클립보드 텍스트에서 붙여넣은 경우는 undo할 때 reserve에 유지하지 않는다.
            vmCuts.CutPaster.SetReserved(targets);
        }
    }
}
