namespace SlackAssist.Contents.ChatReactions.Redmine;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cs.Core.Util;
using Cs.Logging;

using SlackAssist;
using SlackAssist.Fremawork.Redmines;
using SlackAssist.Fremawork.Slack;
using SlackAssist.Fremawork.Slack.MessageHandlerBase;

using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;
using static SlackAssist.Fremawork.Redmines.Enums;

internal sealed class UpdateRedmineIssue : RegexReaction
{
    private readonly string command = string.Join("|", EnumUtil<IssueStatusType>.GetValues());
    public override string Pattern => @$"\[레드마인 #(\d+)\ ({this.command})(\s|])?(\S*)]";
    private string DebugName => $"[UpdateStatusRedmine]";

    public override Block GetIntroduceBlock()
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"패턴 : `[레드마인 #<<일감번호>> 상태 담당자]`");
            writer.WriteLine($"효과 : 일감의 상태를 변경합니다. 담당자를 적으면 일감의 담당자도 함께 변경합니다.");
            writer.WriteLine($"예시 : *[레드마인 #29 해결 주시윤]*");
        }

        return new SectionBlock { Text = new Markdown(sb.ToString()) };
    }

    public override async Task Process(ISlackApiClient slack, TargetEventRecord eventRecord)
    {
        var targetEvent = eventRecord.TargetEvent;
        string userName = await targetEvent.GetUserNameAsync(slack);
        string userRealName = await targetEvent.GetUserRealNameAsync(slack);
        string channelName = await eventRecord.OriginalEvent.GetChannelNameAsync(slack);

        var issueId = eventRecord.RegexMatch!.Groups[1].Value;
        var issueStatusString = eventRecord.RegexMatch!.Groups[2].Value ?? string.Empty;
        var issueAssignedToAfter = eventRecord.RegexMatch!.Groups[4].Value ?? string.Empty;
        Log.Debug($"{this.DebugName} message:{targetEvent.Text} issueId:{issueId} issueState:{issueStatusString} issueToAssigned:{issueAssignedToAfter} userName:{userName} channelName:{channelName}");
        var teamId = targetEvent.Team;
        var channelId = eventRecord.OriginalEvent.Channel;
        var slackResponse = new Message
        {
            IconEmoji = ":redmine:",
            Username = "Redmine",
            Text = string.Empty,
            Channel = channelId,
            ThreadTs = targetEvent.Ts,
        };

        if (Enum.TryParse<IssueStatusType>(issueStatusString, out var issueStatus) == false)
        {
            slackResponse.Text = $":x: 레드마인에서 \"{issueStatusString}\"상태를 찾을 수 없습니다";
            await slack.Chat.PostEphemeral(targetEvent.User, slackResponse);
            return;
        }

        var issue = await Redmine.Instance.GetIssue(issueId);
        if (issue is null)
        {
            slackResponse.Text = $":x: 레드마인 일감 {issueId}번을 찾을 수 없습니다";
            await slack.Chat.PostEphemeral(targetEvent.User, slackResponse);
            return;
        }

        var issueStatusBefore = issue.Status.Name;
        if (issueStatusBefore == issueStatusString && string.IsNullOrEmpty(issueAssignedToAfter))
        {
            slackResponse.Text = $":x: 레드마인 일감 {issueId}번은 이미 \"{issueStatusString}\"상태 입니다";
            await slack.Chat.PostEphemeral(targetEvent.User, slackResponse);
            return;
        }

        var issueAssignedBefore = issue.AssignedTo?.Name ?? string.Empty;

        // --------------- 레드마인 댓글에 기록할 Notes을 생성한다
        var messageUri = await slack.Chat.GetPermalink(channelId, targetEvent.Ts);

        var sb = new StringBuilder();
        var encodedText = HttpUtility.HtmlEncode(targetEvent.Text);
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"슬랙에서 해당 일감의 상태가 변경되었습니다.");
            writer.WriteLine($" - 채널 : [#{channelName}](slack://channel?team={teamId}&id={channelId})");
            writer.WriteLine($" - 작성자 : {userRealName} ([@{userName}](slack://user?team={teamId}&id={targetEvent.User}))");
            writer.WriteLine($" - 메세지 : [{encodedText}]({messageUri.Permalink})");
            writer.WriteLine($" - 상태 : [{issueStatusBefore}] -> [{issueStatusString}]");
        }

        await using (var updater = Redmine.Instance.GetIssueUpdater(issue))
        {
            //------ 댓글 달기 (상태 동일하고, 담당자만 바뀌는 경우 422 Unprocessable Entity 발생 그래서 댓글 부터 단다.)
            updater.AddNotes(sb.ToString());

            //------ 일감 수정
            await updater.ChangeStaus(issueStatus);
            if (string.IsNullOrEmpty(issueAssignedToAfter) == false)
            {
                updater.ChangeAssignUser(issueAssignedToAfter);
            }
            else if (issueStatus == IssueStatusType.해결)
            {
                updater.ChangeAssignUser(issue.Author);
            }
            
            issueAssignedToAfter = issue.AssignedTo?.Name ?? string.Empty;
        }

        // --------------- 슬랙에 스레드 댓글로 처리내용을 응답한다
        var issueUrl = Redmine.Instance.Host.AppendToURL("issues", issueId);
        //// <url|text> 형식. 참고 : https://api.slack.com/reference/surfaces/formatting#linking-urls

        // 담당자 변경 텍스트
        string assignedToText = string.Empty;
        if (string.IsNullOrEmpty(issueAssignedToAfter) == false && issueAssignedBefore != issueAssignedToAfter)
        {
            assignedToText = $"\n- 담당자 : {issueAssignedBefore} -> {issueAssignedToAfter}";
        }

        slackResponse.Text = $"<{issueUrl}|[{issueId}] {issue.GetEncodedSubject()}>일감을 수정했습니다.\n- 상태 : {issueStatusBefore} -> {issueStatusString}{assignedToText}";
        await slack.Chat.PostMessage(slackResponse);
    }
}