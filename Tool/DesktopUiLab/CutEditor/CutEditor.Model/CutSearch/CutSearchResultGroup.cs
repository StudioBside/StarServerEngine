namespace CutEditor.Model.CutSearch;

using System;
using System.Collections.Generic;

public sealed class CutSearchResultGroup
{
    private readonly List<CutSearchResult> results = new();

    public CutSearchResultGroup(string cutsceneName)
    {
        this.CutsceneName = cutsceneName;
    }

    public string CutsceneName { get; }

    public IEnumerable<CutSearchResult> Results => this.results;

    public void AddResult(CutSearchResult result)
    {
        this.results.Add(result);
    }
}
