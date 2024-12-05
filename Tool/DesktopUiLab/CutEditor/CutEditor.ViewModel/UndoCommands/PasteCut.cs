namespace CutEditor.ViewModel.UndoCommands;

using System;
using Cs.Logging;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.ViewModel.UndoCommands.PasteCut;

internal sealed class PasteCut(
    VmCuts vmCuts,
    IReadOnlyList<VmCut> targets,
    int positionIndex,
    PasteDirection direction) : IDormammu
{
    public enum PasteDirection
    {
        Upside,
        Downside,
    }

    public bool ReserveOnUndo { get; init; } = true;

    public static PasteCut? Create(VmCuts vmCuts, PasteDirection direction)
    {
        if (vmCuts.CutPaster.Reserved.Count == 0)
        {
            return null;
        }

        if (vmCuts.SelectedCuts.Count != 1)
        {
            Log.Warn($"컷을 붙여넣으려면 현재 선택된 컷이 하나여야 합니다. 현재 선택된 컷 수: {vmCuts.SelectedCuts.Count}");
            return null;
        }

        int positionIndex = vmCuts.Cuts.IndexOf(vmCuts.SelectedCuts[0]);
        return new PasteCut(vmCuts, vmCuts.CutPaster.Reserved.ToArray(), positionIndex, direction);
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

        if (this.ReserveOnUndo)
        {
            // 클립보드 텍스트에서 붙여넣은 경우는 undo할 때 reserve에 유지하지 않는다.
            vmCuts.CutPaster.SetReserved(targets);
        }
    }
}
