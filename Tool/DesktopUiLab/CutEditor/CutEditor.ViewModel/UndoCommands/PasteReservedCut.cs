namespace CutEditor.ViewModel.UndoCommands;

using System;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.ViewModel.VmCutPaster;

internal sealed class PasteReservedCut(
    VmCuts vmCuts,
    IReadOnlyList<VmCut> targets,
    int positionIndex,
    PasteDirection direction) : IDormammu
{
    public static PasteReservedCut? Create(VmCuts vmCuts, PasteDirection direction)
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

        return new PasteReservedCut(vmCuts, vmCuts.CutPaster.Reserved.ToArray(), positionIndex, direction);
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

        vmCuts.SelectedCuts.Clear();
        vmCuts.SelectedCuts.Add(vmCuts.Cuts[selectedIndex]);

        var controller = vmCuts.Services.GetRequiredService<ICutsListController>();
        controller.FocusElement(selectedIndex);

        vmCuts.CutPaster.SetReserved(targets);
    }
}
