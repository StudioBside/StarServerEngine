namespace WikiTool.Core.Transform.CustomReplacer;

using System.Text.RegularExpressions;
using Cs.Logging;
using Html2Markdown.Replacement;

public sealed class PageLinkReplacer : CustomReplacer
{
    // regex 추츨 내용을 중간에 한 번 변환해서 사용해야 하기 때문에
    // PatternReplacer를 바로 사용할 순 없고 Custom으로 처리.
    public PageLinkReplacer()
    {
        this.CustomAction = this.Execute;
    }

    private string Execute(string html)
    {
        // using regex to : [title](url) -> [title|url]
        var regex = new Regex(@"\[(?<title>[^\]]+)\]\((?<url>[^\)]+)\)");
        return regex.Replace(html, match =>
        {
            var title = match.Groups["title"].Value;
            var url = match.Groups["url"].Value;

            if (url.StartsWith("http"))
            {
                return $"[{title}|{url}]"; // 외부 링크는 그대로 둔다.
            }

            var wjPage = WikiJsController.Instance.GetByPath(url);
            if (wjPage == null)
            {
                Log.Warn($"Page not found: {url}");
                return $"[{title}|{url}]";
            }

            return $"[{title}|{wjPage.GetUniqueTitle()}]";
        });
    }
}
