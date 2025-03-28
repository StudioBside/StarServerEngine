﻿namespace CutEditor.ViewModel.UndoCommands;

using System;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

internal class DeleteCuts(VmCuts vmCuts, IReadOnlyList<VmCut> targets) : IDormammu
{
    private readonly IReadOnlyList<VmCut> cuts = vmCuts.Cuts.ToArray();

    protected VmCuts VmCuts => vmCuts;
    protected IReadOnlyList<VmCut> Targets => targets;

    public static DeleteCuts? Create(VmCuts vmCuts)
    {
        if (vmCuts.SelectedCuts.Count == 0)
        {
            return null;
        }

        return new DeleteCuts(vmCuts, vmCuts.SelectedCuts.ToArray());
    }

    public static DeleteCuts? Create(VmCuts vmCuts, VmCut deleteTarget)
    {
        return new DeleteCuts(vmCuts, [deleteTarget]);
    }

    public virtual void Redo()
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

        if (selectedIndex < vmCuts.Cuts.Count)
        {
            var selected = vmCuts.Cuts[selectedIndex];
            vmCuts.SelectedCuts.Clear();
            vmCuts.SelectedCuts.Add(vmCuts.Cuts[selectedIndex]);

            var controller = vmCuts.Services.GetRequiredService<ICutsListController>();
            controller.FocusElement(selectedIndex);
        }
    }

    public virtual void Undo()
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

        // 제거되었다 되살아난 항목을 다시 선택
        vmCuts.SelectedCuts.Clear();
        foreach (var cut in targets)
        {
            vmCuts.SelectedCuts.Add(cut);
        }
    }
}
