namespace WikiTool.Core;

using Html2Markdown.Replacement;

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
            Replacement = "{color:yellow}",
        });
        
        this.AddReplacer(new PatternReplacer
        {
            Pattern = "</mark>",
            Replacement = "{color}",
        });
        
        // [title](url) -> [title|url]
        this.AddReplacer(new PatternReplacer
        {
            Pattern = @"\[(?<title>[^\]]+)\]\((?<url>[^\)]+)\)",
            Replacement = @"[${title}|${url}]",
        });
    }
}
