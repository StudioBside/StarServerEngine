namespace WikiTool.Core.Transform;

using Html2Markdown.Replacement;
using Html2Markdown.Scheme;
using WikiTool.Core.Transform.Replacement;

public sealed class ConvertingScheme : Markdown
{
    public ConvertingScheme()
    {
        // 기본 규칙들보다 먼저 실행되어야 하는 것은 insert로.
        this.ReplacerCollection.Insert(index: 0, new MyPatternComposite());

        // AddReplacementGroup(this.ReplacerCollection, new ReplacementGroup());
        this.ReplacerCollection.Add(new FigureTagRemover());
        this.ReplacerCollection.Add(new MarkTagReplacer());

        var removeTargets = this.ReplacerCollection.OfType<HeadingTagReplacer>().ToArray();
        foreach (var target in removeTargets)
        {
            this.ReplacerCollection.Remove(target);
        }

        AddReplacementGroup(this.ReplacerCollection, new NewHeadingReplacementGroup());
    }
}
