namespace WikiTool.Core;

using Html2Markdown.Replacement;

public sealed class WikiHeadingReplacer : CustomReplacer
{
    public WikiHeadingReplacer()
    {
        // ## -> h2., ### -> h3., ..., ###### -> h6.
        this.CustomAction = html =>
        {
            return html.Replace("## ", $"h2. ")
                .Replace("### ", $"h3. ")
                .Replace("#### ", $"h4. ")
                .Replace("##### ", $"h5. ")
                .Replace("###### ", $"h6. ");
        };
    }
}
