namespace SlackAssist.Contents.SlashCommands.Redmine;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cs.Core.Util;
using Cs.Messaging;
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

internal sealed class VersionSummary : ISlashSubCommand, IWorkflowCommand
{
    public string Command => SlashCommandHandler.BuildCommand("redmine");
    public IEnumerable<string> CommandLiterals { get; set; } = new[] { "summary", "요약" };

    public Block GetIntroduceBlock()
    {
        using var builder = new MarkdownBuilder();
        builder.WriteLine($"명령 : {string.Join(", ", this.CommandLiterals.Select(e => $"`{e}`"))} :redmine:");
        builder.WriteLine($"효과 : 검색어로 목표버전을 찾고 일감의 진행상황을 요약해서 표시합니다.");
        builder.WriteLine($"문법 : <서브명령> <목표 검색어>");
        builder.WriteLine($"예시 : *{this.Command} summary 1월*");

        return builder.FlushToSectionBlock();
    }

    public Task<Message> Process(ISlackApiClient slack, SlashCommand command, IReadOnlyList<string> arguments)
    {
        BackgroundJob.Execute(async () =>
        {
            var message = await this.Process(slack, arguments);
            message.Channel = command.ChannelId;
            await slack.Chat.PostEphemeral(command.UserId, message);
        });

        return Task.FromResult(new Message { Text = ":loading:" });
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
            result.Blocks.Add(sectionBlock);

            var actionsBlock = new ActionsBlock();
            actionsBlock.Elements.Add(SummaryDetailButton.Create("Detail (Me)", searchKeyword));
            actionsBlock.Elements.Add(SummaryDetailChannelButton.Create("Detail (Ch.)", searchKeyword));
            result.Blocks.Add(actionsBlock);
        }

        return result;
    }
}
