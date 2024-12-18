namespace CutEditor.Model.CutSearch;

public sealed class CutSearchResult
{
    public CutSearchResult(CutScene cutScene, Cut cut, IEnumerable<string> keywords)
    {
        this.CutScene = cutScene;
        this.Cut = cut;
        this.Keywords = keywords;
    }

    public CutScene CutScene { get; }
    public Cut Cut { get; }
    public IEnumerable<string> Keywords { get; }
}
