namespace CutEditor.ViewModel.UndoCommands;

using System;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.ViewModel.VmCutPaster;

internal sealed class PasteCopiedCut(
    VmCuts vmCuts,
    IReadOnlyList<VmCut> targets,
    int positionIndex,
    PasteDirection direction) : IDormammu
{
    public static PasteCopiedCut? Create(VmCuts vmCuts, IReadOnlyList<CutPreset> presets, PasteDirection direction)
    {
        if (presets.Count == 0)
        {
            return null;
        }

        int positionIndex = vmCuts.Cuts.Count - 1; // 선택컷이 없으면 마지막에 붙인다.
        if (vmCuts.SelectedCuts.Count > 0)
        {
            positionIndex = vmCuts.Cuts.IndexOf(vmCuts.SelectedCuts[0]);
        }

        var targets = presets.Select(e => e.CreateCut(vmCuts.NewCutUid()))
            .Select(e => new VmCut(e, vmCuts))
            .ToList();

        return new PasteCopiedCut(vmCuts, targets, positionIndex, direction);
    }

    public void Redo()
    {
        int index = direction switch
        {
            PasteDirection.Upside => positionIndex,
            PasteDirection.Downside => positionIndex + 1,
            _ => throw new InvalidOperationException(),
        };

        vmCuts.SelectedCuts.Clear();
        foreach (var target in targets)
        {
            vmCuts.Cuts.Insert(index, target);
            index++;

            vmCuts.SelectedCuts.Add(target);
        }

        var controller = vmCuts.Services.GetRequiredService<ICutsListController>();
        controller.FocusElement(index);
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
    }
}
