namespace SlackAssist.Contents.ChatReactions.Redmine;

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

internal sealed class LinkRedmineIssue : RegexReaction
{
    public override string Pattern => @"\[레드마인 #(\d+)\]";
    private string DebugName => $"[LinkRedmine]";

    public override Block GetIntroduceBlock()
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"패턴 : `[레드마인 #<<일감번호>>]`");
            writer.WriteLine($"효과 : 해당 번호의 레드마인 일감과 슬랙의 메시지에 상호 링크를 달아 연결합니다.");
            writer.WriteLine($"예시 : *[레드마인 #29]* 일감 논의용 스레드 입니다");
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
        Log.Debug($"{this.DebugName} message:{targetEvent.Text} issueId:{issueId} userName:{userName} channelName:{channelName}");
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

        var issue = await Redmine.Instance.GetIssue(issueId);
        if (issue is null)
        {
            slackResponse.Text = $":x: 레드마인 일감 {issueId}번을 찾을 수 없습니다";
            await slack.Chat.PostMessage(slackResponse);
            return;
        }

        // --------------- 레드마인 일감에 기록할 description을 생성한다
        // 1672997672.713269 -> slack://studiobsidedev.slack.com/archives/C04GPDNTS87/p1672997672713269
        var messageUri = await slack.Chat.GetPermalink(channelId, targetEvent.Ts);

        var sb = new StringBuilder();
        var encodedText = HttpUtility.HtmlEncode(targetEvent.Text);
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"슬랙에서 해당 일감의 진행 상태를 공유하는 스레드가 열렸습니다");
            writer.WriteLine($" - 채널 : [`#{channelName}`](slack://channel?team={teamId}&id={channelId})");
            writer.WriteLine($" - 작성자 : {userRealName} ([@{userName}](slack://user?team={teamId}&id={targetEvent.User}))");
            writer.WriteLine($" - 메시지 : [{encodedText}]({messageUri.Permalink})");
            writer.WriteLine($" - 생성시각 : {ServiceTime.FromUtcTime(targetEvent.Timestamp)}");
        }

        await using (var updater = Redmine.Instance.GetIssueUpdater(issue))
        {
            updater.AddLinkDescription(sb.ToString());
        }

        // --------------- 슬랙에 스레드 댓글로 처리내용을 응답한다
        var issueUrl = Redmine.Instance.Host.AppendToURL("issues", issueId);
        //// <url|text> 형식. 참고 : https://api.slack.com/reference/surfaces/formatting#linking-urls
        slackResponse.Text = $"<{issueUrl}|[{issueId}] {issue.GetEncodedSubject()}>에 이 스레드 링크를 연결했습니다.";
        await slack.Chat.PostMessage(slackResponse);
    }
}
