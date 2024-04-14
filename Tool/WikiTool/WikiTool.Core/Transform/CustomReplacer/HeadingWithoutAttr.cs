namespace WikiTool.Core.Transform.CustomReplacer;

using System.Text.RegularExpressions;
using Cs.Logging;
using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class HeadingWithoutAttr : CustomReplacer
{
    public HeadingWithoutAttr()
    {
        this.CustomAction = this.Execute;
    }

    private string Execute(string html)
    {
        // remove all attributes from heading tags
        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4|//h5|//h6");
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
