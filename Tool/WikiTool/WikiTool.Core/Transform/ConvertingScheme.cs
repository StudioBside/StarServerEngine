namespace WikiTool.Core.Transform;

using Html2Markdown.Replacement;
using Html2Markdown.Scheme;
using WikiTool.Core.Transform.Replacement;

public sealed class ConvertingScheme : Markdown
{
    public ConvertingScheme()
    {
        // AddReplacementGroup(this.ReplacerCollection, new ReplacementGroup());
        this.ReplacerCollection.Add(new FigureTagRemover());
        this.ReplacerCollection.Add(new MarkTagReplacer());
        this.ReplacerCollection.Add(new WikiHeadingReplacer());

        var removeTargets = this.ReplacerCollection.OfType<HeadingTagReplacer>().ToArray();
        foreach (var target in removeTargets)
        {
            this.ReplacerCollection.Remove(target);
        }

        AddReplacementGroup(this.ReplacerCollection, new NewHeadingReplacementGroup());
    }
}
