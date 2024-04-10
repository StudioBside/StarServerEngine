namespace WikiTool.Core.Transform;

using System.Collections.Generic;
using Html2Markdown.Replacement;
using WikiTool.Core.Transform.Replacement;

// 작성했으나 현재 사용 중이지는 않음.
internal sealed class ReplacementGroup : IReplacementGroup
{
    public IEnumerable<IReplacer> Replacers()
    {
        yield return new FigureTagRemover();
    }
}
