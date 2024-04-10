namespace WikiTool.Core;

using Html2Markdown.Replacement;

public class NewHeadingTagReplacer : CompositeReplacer
{
    public NewHeadingTagReplacer(Heading heading)
    {
        var headingNumber = (int)heading;

        // 변환 결과 제목이 보기에 너무 작아서, heading level을 하나씩 올립니다.
        int targetNumber = Math.Max(1, headingNumber - 1);

        this.AddReplacer(new PatternReplacer
        {
            Pattern = $"</h{headingNumber}>",
            Replacement = Environment.NewLine + Environment.NewLine,
        });
        
        this.AddReplacer(new PatternReplacer
        {
            Pattern = $"<h{headingNumber}[^>]*>",
            Replacement = Environment.NewLine + Environment.NewLine + $"h{targetNumber}. ",
        });
    }
}
