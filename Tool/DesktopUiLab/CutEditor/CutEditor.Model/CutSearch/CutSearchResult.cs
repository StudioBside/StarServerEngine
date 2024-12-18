namespace CutEditor.Model.CutSearch;

public sealed class CutSearchResult
{
    public CutSearchResult(CutScene cutScene, Cut cut, IEnumerable<string> keywords)
    {
        this.CutScene = cutScene;
        this.Cut = cut;
        this.Keywords = keywords;

        if (this.Cut.Choices.Count > 0)
        {
            this.ResultText = $"[선택지] {string.Join(" / ", this.Cut.Choices.Select(e => e.Text))}";
        }
        else
        {
            this.ResultText = this.Cut.UnitTalk.Korean;
        }
    }

    public CutScene CutScene { get; }
    public Cut Cut { get; }
    public IEnumerable<string> Keywords { get; }
    public string ResultText { get; }
}
