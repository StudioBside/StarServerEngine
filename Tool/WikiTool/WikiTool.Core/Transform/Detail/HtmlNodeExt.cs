namespace WikiTool.Core.Transform.Detail;

using HtmlAgilityPack;

internal static class HtmlNodeExt
{
    public static void ReplaceNode(this HtmlNode node, string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            node.ParentNode.RemoveChild(node); // 제거만 하면 된다.
            return;
        }

        node.ReplaceNodeWithString(markdown);
    }

    public static void ReplaceNodeWithString(this HtmlNode node, string content)
    {
        HtmlNode htmlNode = HtmlNode.CreateNode("<p></p>");
        htmlNode.InnerHtml = content;
        HtmlNode refChild = node;
        foreach (HtmlNode item in htmlNode.ChildNodes)
        {
            node.ParentNode.InsertAfter(item, refChild);
            refChild = item;
        }

        node.Remove();
    }
}
