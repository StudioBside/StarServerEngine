namespace CutEditor.Model.CutSearch;

using System;
using System.Collections.Generic;

public sealed class CutSearchResult
{
    private readonly Cut cut;

    public CutSearchResult(Cut cut)
    {
        this.cut = cut;
    }
}
