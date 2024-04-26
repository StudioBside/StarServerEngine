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
        //// <ac:image>
        //// <ri:url ri:value="http://confluence.atlassian.com/images/logo/confluence_48_trans.png" />
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
            // var replace = $"<ac:image><ri:url ri:value=\"{srcValue}\" /></ac:image>";
            var replace = $"<ac:image><ri:attachment ri:filename=\"test.jpg\" /></ac:image>";
            node.ReplaceNode(replace);
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
