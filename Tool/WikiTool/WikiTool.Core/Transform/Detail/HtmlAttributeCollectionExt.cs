namespace WikiTool.Core.Transform.Detail;

using HtmlAgilityPack;

internal static class HtmlAttributeCollectionExt
{
    public static string GetAttributeOrEmpty(this HtmlAttributeCollection collection, string attributeName)
    {
        return collection[attributeName]?.Value ?? string.Empty;
    }
}
