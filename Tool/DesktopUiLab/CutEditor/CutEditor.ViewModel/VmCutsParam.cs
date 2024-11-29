namespace CutEditor.ViewModel;

using CutEditor.Model;

public sealed record VmCutsParam
{
    public CutScene? CutScene { get; init; }
    public string? NewFileName { get; init; }
    public long CutUid { get; init; }
}