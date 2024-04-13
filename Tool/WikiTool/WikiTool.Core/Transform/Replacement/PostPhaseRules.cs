namespace WikiTool.Core;

using Html2Markdown.Replacement;
using WikiTool.Core.Transform.CustomReplacer;

public sealed class PostPhaseRules : CompositeReplacer
{
    public PostPhaseRules()
    {
        // figure tag를 삭제.
        this.AddReplacer(new PatternReplacer
        {
            Pattern = "</?figure[^>]*>",
            Replacement = string.Empty,
        });
        
        // ‘<mark class="marker-yellow">Insert Assets</mark>’ -> ‘{color:yellow}Insert Assets{color}’
        this.AddReplacer(new PatternReplacer
        {
            Pattern = "<mark class=\"marker-yellow\">",
            Replacement = "<span style=\"color: rgb(255,255,0);\">",
        });
        
        this.AddReplacer(new PatternReplacer
        {
            Pattern = "</mark>",
            Replacement = "</span>",
        });
        
        this.AddReplacer(new PageLinkReplacer());
    }
}
