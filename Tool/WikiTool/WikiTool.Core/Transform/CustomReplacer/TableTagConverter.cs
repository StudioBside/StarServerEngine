namespace WikiTool.Core.Transform.CustomReplacer;

using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Text.RegularExpressions;
using Cs.Logging;
using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class TableTagConverter : CompositeReplacer
{
    public TableTagConverter()
    {
        this.AddReplacer(new CustomReplacer { CustomAction = this.Execute });
    }

    private string Execute(string html)
    {
        // remove all attributes from heading tags
        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//tr|//th|//td");
        if (htmlNodeCollection == null)
        {
            return html;
        }

        foreach (var node in htmlNodeCollection)
        {
            // remove style attribute
            node.Attributes.Remove("style");
        }

        // change thead data to normal tr in tbody tag
        // make new tr data to first tr in tbody tag
        HtmlNodeCollection theadCollection = htmlDocument.DocumentNode.SelectNodes("//thead");
        if (theadCollection != null)
        {
            foreach (var thead in theadCollection)
            {
                var tbody = thead.ParentNode.SelectSingleNode("tbody");
                if (tbody == null)
                {
                    tbody = htmlDocument.CreateElement("tbody");
                    thead.ParentNode.AppendChild(tbody);
                }

                var trCollection = thead.SelectNodes("tr");
                if (trCollection != null)
                {
                    foreach (var tr in trCollection)
                    {
                        // change th child to td
                        var thCollection = tr.SelectNodes("th");
                        if (thCollection != null)
                        {
                            foreach (var th in thCollection)
                            {
                                th.ReplaceNode($"<td>{th.InnerHtml}</td>");
                            }
                        }

                        tbody.PrependChild(tr);
                    }
                }

                thead.Remove();
            }
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
