namespace CutEditor.Model;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CutEditor.Model.Detail;

public sealed class L10nMapping : ObservableObject
{
    private readonly List<CutOutputExcelFormat> importedList = new();

    public L10nMapping(Cut cut)
    {
        this.Cut = cut;
    }

    public Cut Cut { get; }

    public void Add(CutOutputExcelFormat cutOutputExcelFormat)
    {
        this.importedList.Add(cutOutputExcelFormat);
    }
}
