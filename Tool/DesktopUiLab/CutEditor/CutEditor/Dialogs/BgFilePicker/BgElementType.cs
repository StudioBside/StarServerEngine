namespace CutEditor.Dialogs.BgFilePicker;

using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Shared.Interfaces;

public sealed class BgElementType : ISearchable
{
    public BgElementType(string fileName)
    {
        this.FullPath = Path.GetFullPath(fileName);
        this.FileNameOnly = Path.GetFileName(fileName);
    }

    public string FileNameOnly { get; }
    public string FullPath { get; }
    public string Category { get; init; } = string.Empty;

    public bool IsTarget(string keyword) => this.FileNameOnly.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    public override string ToString() => this.FileNameOnly;
}
