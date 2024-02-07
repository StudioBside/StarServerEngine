namespace SlackAssist.Contents.SlashCommands.Redmine;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs.Core.Util;
using SlackAssist.Fremawork.Redmines;
using SlackAssist.Fremawork.Slack;
using SlackAssist.SlackNetHandlers;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal sealed class IssueLink : ISlashSubCommand
{
    public string Command => SlashCommandHandler.BuildCommand("redmine");
    public IEnumerable<string> CommandLiterals { get; } = new[] { "link", "연결" };

    public Block GetIntroduceBlock()
    {
        using var builder = new MarkdownBuilder();
        builder.WriteLine($"명령 : {string.Join(", ", this.CommandLiterals.Select(e => $"`{e}`"))} :redmine:");
        builder.WriteLine($"효과 : 명령을 수행하는 채널 정보를 레드마인 일감 본문에 기록합니다.");
        builder.WriteLine($"문법 : <서브명령> <일감번호1..>");
        builder.WriteLine($"예시 : *{this.Command} link 223 224 225 226*");

        return builder.FlushToSectionBlock();
    }

    public async Task<Message> Process(ISlackApiClient slack, SlashCommand command, IReadOnlyList<string> arguments)
    {
        if (arguments.Any() == false)
        {
            return new Message { Text = "채널을 연결할 일감 번호를 적어야 합니다. (ex: 223 224 225)" };
        }

        var failedList = new List<string>();
        var successList = new List<IssueData>();
        foreach (var argument in arguments)
        {
            var issue = await Redmine.Instance.GetIssue(argument);
            if (issue is null)
            {
                failedList.Add(argument);
                continue;
            }

            // --------------- 레드마인 일감 본문에 슬랙 채널 정보를 기록한다.
            var message = $"슬랙의 [`#{command.ChannelName}`](slack://channel?team={command.TeamId}&id={command.ChannelId}) 채널에서 해당 일감의 진행 사항을 공유합니다.";
            await using (var updater = Redmine.Instance.GetIssueUpdater(issue))
            {
                updater.AddLinkDescription(message);
            }

            successList.Add(new(argument, issue.GetEncodedSubject()));
        }

        if (successList.Any())
        {
            await ShareToChannel(slack, command.ChannelId, successList);
        }

        var result = new Message();
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"`{command.Command} {command.Text}` 명령 실행 결과");
            writer.WriteLine($"성공 : {successList.Count} 건");
            if (failedList.Any())
            {
                writer.WriteLine($"실패 : {failedList.Count} 건 - {string.Join(", ", failedList.Select(e => $"`{e}`"))}");
            }
        }

        result.Blocks.Add(new SectionBlock { Text = new Markdown(sb.ToString()) });
        return result;
    }

    // ----------------------------------------------------------------------------------------------------
    private static async Task ShareToChannel(ISlackApiClient slack, string channelId, List<IssueData> successList)
    {
        var slackResponse = new Message
        {
            IconEmoji = ":redmine:",
            Username = "Redmine",
            Text = string.Empty,
            Channel = channelId,
        };

        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"레드마인 일감에 해당 채널의 링크를 연결했습니다.");
            foreach (var data in successList)
            {
                var issueUrl = Redmine.Instance.Host.AppendToURL("issues", data.Id);
                writer.WriteLine($"• <{issueUrl}|[{data.Id}] {data.Subject}>");
            }
        }

        slackResponse.Text = sb.ToString();
        await slack.Chat.PostMessage(slackResponse);
    }

    private sealed record IssueData(string Id, string Subject);
}
