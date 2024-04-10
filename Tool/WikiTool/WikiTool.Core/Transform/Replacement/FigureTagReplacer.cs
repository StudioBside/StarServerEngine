namespace WikiTool.Core.Transform.Replacement;

using Html2Markdown.Replacement;

public sealed class FigureTagReplacer : PatternReplacer
{
    public FigureTagReplacer()
    {
        // figure tag를 삭제.
        this.Pattern = "</?figure[^>]*>";
        this.Replacement = string.Empty;
    }
}
