namespace CutEditor.ViewModel.UndoCommands;

using System;
using CutEditor.Model;
using Du.Core.Interfaces;
using static CutEditor.ViewModel.Enums;

internal sealed class NewCut(VmCuts vmCuts, int index, VmCut newCut) : IDormammu
{
    public static NewCut Create(VmCuts vmCuts, CutDataType cutDataType)
    {
        int index = vmCuts.Cuts.Count;
        if (vmCuts.SelectedCuts.Count > 0)
        {
            index = vmCuts.Cuts.IndexOf(vmCuts.SelectedCuts[^1]) + 1;
        }

        var cut = new Cut(vmCuts.UidGenerator.Generate());
        var vmCut = new VmCut(cut, vmCuts.Services);

        if (cutDataType == CutDataType.Branch)
        {
            vmCut.DataType = cutDataType;
            vmCut.AddChoiceOptionCommand.Execute(parameter: null);
        }

        return new NewCut(vmCuts, index, vmCut);
    }

    public void Redo()
    {
        vmCuts.Cuts.Insert(index, newCut);
    }

    public void Undo()
    {
        vmCuts.Cuts.Remove(newCut);
    }
}
