namespace WikiTool.Core.Transform.CustomReplacer;

using System.Text.RegularExpressions;
using Cs.Logging;
using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class ListTagConverter : CustomReplacer
{
    public ListTagConverter()
    {
        this.CustomAction = this.Execute;
    }

    private string Execute(string html)
    {
        // remove all attributes from heading tags
        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//ol|//ul");
        if (htmlNodeCollection == null)
        {
            return html;
        }

        foreach (var node in htmlNodeCollection)
        {
            node.Attributes.RemoveAll();
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
