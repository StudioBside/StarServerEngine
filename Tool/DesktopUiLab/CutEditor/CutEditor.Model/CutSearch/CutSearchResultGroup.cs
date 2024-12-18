namespace CutEditor.Model.CutSearch;

using System.Collections.Generic;
using Cs.Logging;

public sealed class CutSearchResultGroup
{
    private readonly List<Cut> results = new();

    public CutSearchResultGroup(CutScene cutScene, IReadOnlyList<Cut> cuts)
    {
        this.CutScene = cutScene;
        this.results.AddRange(cuts);

        Log.Debug($"cutscene:{cutScene.FileName} #result:{this.results.Count}");
    }

    public CutScene CutScene { get; }
    public IEnumerable<Cut> Results => this.results;
}
