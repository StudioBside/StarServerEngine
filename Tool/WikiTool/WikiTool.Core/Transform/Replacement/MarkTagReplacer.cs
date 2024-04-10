namespace WikiTool.Core;

using Html2Markdown.Replacement;

public class MarkTagReplacer : CompositeReplacer
{
    public MarkTagReplacer()
    {
        // ‘<mark class="marker-yellow">Insert Assets</mark>’ -> ‘{color:yellow}Insert Assets{color}’
        this.AddReplacer(new PatternReplacer
        {
            Pattern = "<mark class=\"marker-yellow\">",
            Replacement = "{color:yellow}",
        });
        
        this.AddReplacer(new PatternReplacer
        {
            Pattern = "</mark>",
            Replacement = "{color}",
        });
    }
}