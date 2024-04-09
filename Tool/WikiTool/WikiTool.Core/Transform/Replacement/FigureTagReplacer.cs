namespace WikiTool.Core.Transform.Replacement;

using Html2Markdown.Replacement;

public sealed class FigureTagReplacer : PatternReplacer
{
    public FigureTagReplacer()
    {
        this.Pattern = "</?figure[^>]*>";
        this.Replacement = string.Empty;
    }
}
