namespace WikiTool.Core.Transform.CustomReplacer;

using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class CodeReplacer : CustomReplacer
{
    public CodeReplacer()
    {
        this.CustomAction = this.Execute;
    }

    private string Execute(string html)
    {
        /*
        <ac:structured-macro ac:name=\"code\">
        <ac:plain-text-body>
             <![CDATA[
             ... code ...
            ]]>
        </ac:plain-text-body>
        </ac:structured-macro>
        */
        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//code");
        if (htmlNodeCollection == null)
        {
            return html;
        }

        foreach (var node in htmlNodeCollection)
        {
            if (node.ParentNode.Name == "pre")
            {
                string innerHtml = node.InnerHtml;
                var replace = $"<ac:structured-macro ac:name=\"code\">\n<ac:plain-text-body>\n<![CDATA[{innerHtml}]]>\n</ac:plain-text-body>\n</ac:structured-macro>";
                node.ReplaceNode(replace);
            }
            else
            {
                continue;
            }
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
