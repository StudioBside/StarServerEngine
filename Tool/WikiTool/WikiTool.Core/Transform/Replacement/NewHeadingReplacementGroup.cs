namespace WikiTool.Core;

using Html2Markdown.Replacement;

public class NewHeadingReplacementGroup : IReplacementGroup
{
    private readonly IList<IReplacer> replacements =
    [
        new NewHeadingTagReplacer(Heading.H1),
        new NewHeadingTagReplacer(Heading.H2),
        new NewHeadingTagReplacer(Heading.H3),
        new NewHeadingTagReplacer(Heading.H4),
        new NewHeadingTagReplacer(Heading.H5),
        new NewHeadingTagReplacer(Heading.H6),
    ];

    public IEnumerable<IReplacer> Replacers()
    {
        return this.replacements;
    }
}