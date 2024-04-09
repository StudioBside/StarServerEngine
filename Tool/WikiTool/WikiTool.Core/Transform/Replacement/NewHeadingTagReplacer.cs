namespace WikiTool.Core;

using Html2Markdown.Replacement;

public class NewHeadingTagReplacer : CompositeReplacer
{
    public NewHeadingTagReplacer(Heading heading)
    {
        var headingNumber = (int)heading;
        this.AddReplacer(new PatternReplacer
        {
            Pattern = $"</h{headingNumber}>",
            Replacement = Environment.NewLine + Environment.NewLine,
        });
        
        this.AddReplacer(new PatternReplacer
        {
            Pattern = $"<h{headingNumber}[^>]*>",
            Replacement = Environment.NewLine + Environment.NewLine + $"h{headingNumber}. ",
        });
    }
}
