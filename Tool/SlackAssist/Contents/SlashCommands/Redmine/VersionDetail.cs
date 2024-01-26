namespace SlackAssist.Contents.SlashSlackAssist;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cs.Core.Util;

using global::Redmine.Net.Api.Types;
using SlackAssist.Contents.Detail;
using SlackAssist.Fremawork.Redmines;
using SlackAssist.Fremawork.Slack;
using SlackAssist.Fremawork.Workflow;
using SlackAssist.SlackNetHandlers;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal sealed class RedmineList : ISlashSubCommand, IWorkflowCommand
{
    public SlashCommandCategory Category { get; } = SlashCommandCategory.Redmine;
    public IEnumerable<string> CommandLiterals { get; } = new[] { "list", "목록" };

    public Block GetIntroduceBlock()
    {
        var builder = new MarkdownBuilder();
        builder.WriteLine($"명령 : {string.Join(", ", this.CommandLiterals.Select(e => $"`{e}`"))} :redmine:");
        builder.WriteLine($"효과 : 주어진 검색어로 목표버전을 찾고, 해당 목표가 설정된 일감 목록을 가져옵니다");
        builder.WriteLine($" - `summary` 명령에서 detail 버튼 사용시에도 해당 명령이 수행됩니다.");
        builder.WriteLine($"문법 : <서브명령> <목표 검색어>");
        builder.WriteLine($"예시 : *{this.Category.GetMainCommand()} list 2023년 1월*");

        return builder.FlushToSectionBlock();
    }

    public Task<Message> Process(ISlackApiClient slack, SlashCommand command, IReadOnlyList<string> arguments)
    {
        return this.Process(slack, arguments);
    }

    public async Task<Message> Process(ISlackApiClient slack, IReadOnlyList<string> arguments)
    {
        if (arguments.Any() == false)
        {
            return new Message { Text = "검색어를 적어야 합니다. (ex: 3월)" };
        }

        string searchKeyword = arguments[0];
        var issues = await RedmineController.GetAllIssueInVersion(searchKeyword);
        if (issues.Any() == false)
        {
            return new Message { Text = "일감이 없습니다." };
        }

        // 목표 버전 쓰기
        var result = new Message
        {
            IconEmoji = ":redmine:",
            Username = "Readmine",
        };

        var fixedVersionName = issues[0].FixedVersion.Name;
        result.Blocks.Add(new HeaderBlock { Text = $"{fixedVersionName} ({issues.Count}개)" });

        // 개요 부분. 작성 시각 및 전체 진행률.
        using (var builder = new MarkdownBuilder())
        {
            builder.WriteLine($"기준시각 : {ServiceTime.RecentDefaultString}");
            builder.WriteLine($"전체 진행률 : {issues.ToProgressBar(e => e.IsCompleted())}");
            result.Blocks.Add(builder.FlushToSectionBlock());
        }

        result.Blocks.Add(new DividerBlock());

        // 카테고리별 분류
        var issueCategorys = issues
            .GroupBy(e => e.Category?.Name ?? "미분류")
            .ToDictionary(e => e.Key, e => e.ToList());

        // 내용 추가
        foreach (var category in issueCategorys)
        {
            using var builder = new MarkdownBuilder();
            Issue issue = category.Value.FirstOrDefault(e => e.Project.Id == 1) ?? category.Value[0];
            string redmineUrl;
            var project = Redmine.Instance.GetProjects(issue.Project.Id);
            if (project is null)
            {
                redmineUrl = string.Empty;
            }
            else
            {
                var categoryQueryString = "category_id&op%5Bcategory_id%5D=%21*&f%5B%5D=&c%5B%5D=";
                if (issue.Category is not null)
                {
                    categoryQueryString = $"category_id&op%5Bcategory_id%5D=%3D&v%5Bcategory_id%5D%5B%5D={issue.Category.Id}";
                }

                var versionQueryString = $"fixed_version_id&op%5Bfixed_version_id%5D=%3D&v%5Bfixed_version_id%5D%5B%5D={issue.FixedVersion.Id}";
                redmineUrl = Redmine.Instance.Host.AppendToURL(
                "projects",
                project.Identifier,
                $"""issues?utf8=%E2%9C%93&set_filter=1&sort=id%3Adesc&f%5B%5D={categoryQueryString}&f%5B%5D={versionQueryString}""");
            }

            var categoryTitle = string.IsNullOrEmpty(redmineUrl) ? category.Key : $"<{redmineUrl}|{category.Key}>";
            builder.WriteBoldLine(categoryTitle);
            builder.Write(category.Value.ToProgressBar(e => e.IsCompleted()));
            result.Blocks.Add(builder.FlushToSectionBlock());

            // 버튼 레이아웃이 폰에서 볼 때 너무 과한 면이 있다. 제거 고려중.
            //section.Accessory = ListByGoalDetailButton.Create("more...", searchKeyword, category.Key);
        }

        return result;
    }
}
