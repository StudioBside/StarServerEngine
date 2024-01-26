namespace SlackAssist.Contents.SlashSlackAssist;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cs.Core.Util;

using SlackAssist.Contents.ButtonActions;
using SlackAssist.Contents.Detail;
using SlackAssist.Fremawork.Redmines;
using SlackAssist.Fremawork.Slack;
using SlackAssist.Fremawork.Workflow;
using SlackAssist.SlackNetHandlers;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal sealed class RedmineSummary : ISlashSubCommand, IWorkflowCommand
{
    public SlashCommandCategory Category { get; } = SlashCommandCategory.Redmine;

    public IEnumerable<string> CommandLiterals { get; set; } = new[] { "summary", "요약" };

    public Block GetIntroduceBlock()
    {
        using var builder = new MarkdownBuilder();
        builder.WriteLine($"명령 : {string.Join(", ", this.CommandLiterals.Select(e => $"`{e}`"))} :redmine:");
        builder.WriteLine($"효과 : 검색어로 목표버전을 찾고 일감의 진행상황을 요약해서 표시합니다.");
        builder.WriteLine($"문법 : <서브명령> <목표 검색어>");
        builder.WriteLine($"예시 : *{this.Category.GetMainCommand()} summary 1월*");

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

        string searchKeyword = string.Join(" ", arguments);
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
            var sectionBlock = builder.FlushToSectionBlock();
            sectionBlock.Accessory = GoalSummaryDetailButton.Create("Detail", searchKeyword);
            result.Blocks.Add(sectionBlock);
        }

        return result;
    }
}
