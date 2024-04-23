namespace WikiTool.Core.Transform.CustomReplacer;

using Html2Markdown.Replacement;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

public sealed class VideoTagConverter : CompositeReplacer
{
    public VideoTagConverter()
    {
        this.AddReplacer(new CustomReplacer { CustomAction = this.Execute });
    }

    private string Execute(string html)
    {
        /*
        <video controls="">
            <source type="video/webm" src="http://fileserver.bside.com:8081/WebLink/Movies/Tech/Milestone/202206/exception_editor.mkv">
            Sorry, your browser doesn't support embedded videos.
        </video>
        -----
        <p> 미디어 소스 : <a href="http://fileserver.bside.com:8081/WebLink/Movies/Tech/Milestone/202206/exception_editor.mkv">http://fileserver.bside.com:8081/WebLink/Movies/Tech/Milestone/202206/exception_editor.mkv</a> </p>
        */

        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(html);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//video");
        if (htmlNodeCollection == null)
        {
            return html;
        }

        foreach (var node in htmlNodeCollection)
        {
            string srcValue = node.SelectSingleNode("//source").Attributes.GetAttributeOrEmpty("src");
            var replace = $"<p> 미디어 소스 : <a href=\"{srcValue}\">{srcValue}</a> </p>";
            node.ReplaceNode(replace);
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }
}
