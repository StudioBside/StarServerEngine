namespace WikiTool.Core.Transform;

using System;
using System.Text;
using System.Text.RegularExpressions;
using Cs.Core;
using HtmlAgilityPack;
using WikiTool.Core.Transform.Detail;

internal sealed class ContentsConverter
{
    public const int PathPageVersion = 2;
    public const int NodePageVersion = 3;
    private readonly Html2Markdown.Converter htmlConverter = new(new ConvertingScheme());

    public static ContentsConverter Instance => Singleton<ContentsConverter>.Instance;

    public string GetPathPageContents(string title)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"<h2> convert metadata </h2>");
        sb.AppendLine($"<ul>");
        sb.AppendLine($"<li> pageType : path page </li>");
        sb.AppendLine($"<li> convertVersion : {PathPageVersion} </li>");
        sb.AppendLine($"</ul>");

        return sb.ToString();
    }

    public bool IsLatestPathPage(string contents)
    {
        var lines = contents.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            Match match = Regex.Match(line, @"convertVersion\s*:\s*(\d+)");
            if (match.Success)
            {
                var version = int.Parse(match.Groups[1].Value);
                return version == PathPageVersion;
            }
        }

        return false;
    }

    public string GetNodePageContents(string wikiJsContents)
    {
        var sb = new StringBuilder();
        sb.AppendLine(this.htmlConverter.Convert(wikiJsContents));

        sb.AppendLine();
        sb.AppendLine($"<h2> convert metadata </h2>");
        sb.AppendLine($"<ul>");
        sb.AppendLine($"<li> pageType : node page </li>");
        sb.AppendLine($"<li> convertVersion : {NodePageVersion} </li>");
        sb.AppendLine($"</ul>");

        return sb.ToString();
    }

    public List<string> GetAttachmentFileList(string wijiJsContents)
    {
        var result = new List<string>();
        HtmlDocument htmlDocument = HtmlDocumentLoader.Load(wijiJsContents);
        HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//img");
        if (htmlNodeCollection == null)
        {
            return result;
        }

        foreach (var node in htmlNodeCollection)
        {
            string srcValue = node.Attributes.GetAttributeOrEmpty("src");
            if (srcValue.StartsWith("http"))
            {
                continue;
            }

            result.Add(srcValue);
        }

        return result;
    }

    public bool IsLatestNodePage(string contents)
    {
        var lines = contents.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            Match match = Regex.Match(line, @"convertVersion\s*:\s*(\d+)");
            if (match.Success)
            {
                var version = int.Parse(match.Groups[1].Value);
                return version == NodePageVersion;
            }
        }

        return false;
    }
}
