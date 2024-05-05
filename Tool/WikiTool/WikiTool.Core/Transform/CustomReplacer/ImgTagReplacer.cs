namespace WikiTool.Core.Transform.CustomReplacer;

using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class ImgTagReplacer : CustomReplacer
{
    public ImgTagReplacer()
    {
        this.CustomAction = this.Execute;
    }

    private string Execute(string html)
    {
        // 외부 이미지
        //// <ac:image>
        //// <ri:url ri:value="http://confluence.atlassian.com/images/logo/confluence_48_trans.png" />
        //// </ac:image>

        // 내부 이미지
        //// <ac:image>
        //// <ri:attachment ri:filename=\"{srcValue}\" />
        //// </ac:image>

        // 다른 페이지에 있는 이미지
        //// <ac:image>
        //// <ri:attachment ri:filename=\"{srcValue}\" ri:version-at-save=\"1\">
        //// <ri:page ri:content-title=\"MigrationTest2 Home\" ri:version-at-save=\"2\" />
        //// </ri:attachment>
        //// </ac:image>

        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//img");
        if (htmlNodeCollection == null)
        {
            return html;
        }

        foreach (var node in htmlNodeCollection)
        {
            string srcValue = node.Attributes.GetAttributeOrEmpty("src");
            srcValue = Path.GetFileName(srcValue);
            var replace = $"<ac:image><ri:attachment ri:filename=\"{srcValue}\" /></ac:image>";
            //var replace = $"<ac:image><ri:attachment ri:filename=\"test.jpg\"><ri:page ri:content-title=\"MigrationTest2 Home\" /></ri:attachment></ac:image>";
            node.ReplaceNode(replace);
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
