namespace WikiTool.Core.Transform.Replacement;

using Html2Markdown.Replacement;

public sealed class FigureTagRemover : PatternReplacer
{
    public FigureTagRemover()
    {
        // figure tag를 삭제.
        this.Pattern = "</?figure[^>]*>";
        this.Replacement = string.Empty;
    }
}
