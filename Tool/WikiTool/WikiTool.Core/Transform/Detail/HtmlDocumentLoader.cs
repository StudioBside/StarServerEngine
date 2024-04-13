namespace WikiTool.Core.Transform.Detail;

using HtmlAgilityPack;

internal static class HtmlDocumentLoader
{
    public static HtmlDocument Load(string html)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        return htmlDocument;
    }
}
