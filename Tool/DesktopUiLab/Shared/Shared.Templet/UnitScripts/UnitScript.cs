namespace Shared.Templet.UnitScripts;

using System.Collections.Generic;
using Shared.Interfaces;
using Shared.Templet.TempletTypes;

public sealed class UnitScript : ISearchable
{
    private readonly List<Unit> references = new();

    public UnitScript(string path)
    {
        this.FullPath = Path.GetFullPath(path);
        this.FileName = Path.GetFileName(path);
        this.FullText = File.ReadAllText(path);
        this.Loc = this.FullText.Count(c => c == '\n');
    }

    public static IEnumerable<UnitScript> Values => UnitScriptContainer.Instance.Values;
    public IReadOnlyList<Unit> References => this.references;
    public string FileName { get; }
    public string FullPath { get; }
    public string FullText { get; }
    public int Loc { get; }

    public bool IsTarget(string keyword)
    {
        return this.FileName.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    internal void AddReference(Unit unit)
    {
        this.references.Add(unit);
    }
}
