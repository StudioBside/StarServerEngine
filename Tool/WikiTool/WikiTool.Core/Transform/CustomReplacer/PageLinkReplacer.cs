namespace WikiTool.Core.Transform.CustomReplacer;

using System.Text.RegularExpressions;
using Cs.Logging;
using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class PageLinkReplacer : CustomReplacer
{
    public PageLinkReplacer()
    {
        this.CustomAction = this.Execute;
    }

    private string Execute(string html)
    {
        /*
         <ac:link>
         <ri:page ri:content-title="Page Title" />
         <ac:plain-text-link-body>
          <![CDATA[Link to another Confluence Page]]>
         </ac:plain-text-link-body>
         </ac:link> 
        */
        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//a");
        if (htmlNodeCollection == null)
        {
            return html;
        }

        foreach (var node in htmlNodeCollection)
        {
            var url = node.Attributes.GetAttributeOrEmpty("href");
            if (url.StartsWith("http"))
            {
                node.RemoveClass();
                continue;
            }

            var wjPage = WikiJsController.Instance.GetByPath(url);
            if (wjPage == null)
            {
                Log.Warn($"Page not found: {url}");
                continue;
            }

            var title = node.InnerText;
            var replace = $"<ac:link><ri:page ri:content-title=\"{wjPage.GetUniqueTitle()}\" /><ac:plain-text-link-body><![CDATA[{title}]]></ac:plain-text-link-body></ac:link>";
            node.ReplaceNode(replace);
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
