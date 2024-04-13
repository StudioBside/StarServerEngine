namespace WikiTool.Core.Transform;

using Html2Markdown.Replacement;
using Html2Markdown.Scheme;
using WikiTool.Core.Transform.CustomReplacer;

public sealed class ConvertingScheme : AbstractScheme
{
    public ConvertingScheme()
    {
        // 기본 규칙들보다 먼저 실행되어야 하는 것은 앞에서 추가.
        this.ReplacerCollection.Add(new PrePhaseRules());

        //this.ReplacerCollection.Add(new ParagraphTagReplacer()); // <p> 태그 변환
        //this.ReplacerCollection.Add(new ParagraphToBr()); // <p> 태그 변환

        //AddReplacementGroup(this.ReplacerCollection, new TextFormattingReplacementGroup());
        //// AddReplacementGroup(this.ReplacerCollection, new HeadingReplacementGroup());
        //AddReplacementGroup(this.ReplacerCollection, new NewHeadingReplacementGroup());
        //AddReplacementGroup(this.ReplacerCollection, new IllegalHtmlReplacementGroup());
        //AddReplacementGroup(this.ReplacerCollection, new LayoutReplacementGroup());
        //AddReplacementGroup(this.ReplacerCollection, new EntitiesReplacementGroup());

        // 기본 규칙 적용 후 실행되어야 하는 것은 뒤에서 추가.
        this.ReplacerCollection.Add(new PostPhaseRules());
    }
}
