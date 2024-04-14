namespace WikiTool.Core.Transform;

using System;
using System.Text;
using System.Text.RegularExpressions;
using Cs.Core;

internal sealed class ContentsConverter
{
    public const int PathPageVersion = 2;
    public const int NodePageVersion = 2;
    private readonly Html2Markdown.Converter markdownConverter = new(new ConvertingScheme());

    public static ContentsConverter Instance => Singleton<ContentsConverter>.Instance;

    public string GetPathPageContents(string title)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"h2. convert metadata");
        sb.AppendLine($"* pageType : path page");
        sb.AppendLine($"* convertVersion : {PathPageVersion}");

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
        sb.AppendLine(this.markdownConverter.Convert(wikiJsContents));

        sb.AppendLine();
        sb.AppendLine($"<h2> convert metadata </h2>");
        sb.AppendLine($"<ul>");
        sb.AppendLine($"<li> pageType : node page </li>");
        sb.AppendLine($"<li> convertVersion : {NodePageVersion} </li>");
        sb.AppendLine($"</ul>");

        return sb.ToString();
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
