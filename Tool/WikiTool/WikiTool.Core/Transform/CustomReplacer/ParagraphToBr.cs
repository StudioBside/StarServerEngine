namespace WikiTool.Core.Transform.CustomReplacer;

using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class ParagraphToBr : CustomReplacer
{
    public ParagraphToBr()
    {
        this.CustomAction = this.Execute;
    }

    private string Execute(string html)
    {
        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//p");
        if (htmlNodeCollection == null)
        {
            return html;
        }

        foreach (var node in htmlNodeCollection)
        {
            string innerHtml = node.InnerHtml;
            string text = innerHtml.Replace(Environment.NewLine, " ");
            node.ReplaceNode($"<br /><br /> {text} <br />");
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
