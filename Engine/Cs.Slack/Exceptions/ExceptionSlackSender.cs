namespace Cs.Slack.Exceptions;

using System.Collections.Generic;
using System.Drawing;
using Cs.Exception;
using Cs.Logging;
using Cs.Slack.Elements;

public sealed class ExceptionSlackSender : IExceptionSlackSender
{
    private readonly SlackEndpoint[] slackEndponts;
   
    public ExceptionSlackSender(SlackEndpoint[] slackEndponts)
    {
        List<SlackEndpoint> validEndpoints = new();
        foreach (var endpoint in slackEndponts)
        {
            if (SlackGlobalSetting.TryGetChannelId(endpoint.Channel, out var channelId) == false)
            {
                Log.Error($"크래시 리포트 할 슬랙 채널의 아이디를 찾지 못했습니다 channel:{endpoint.Channel}");
                continue;
            }

            validEndpoints.Add(new SlackEndpoint(endpoint.Token, channelId));
        }

        this.slackEndponts = validEndpoints.ToArray();
    }

    public void SendCrashReport(string title, string text)
    {
        using var writer = new SlackWriter(this.slackEndponts, SlackGlobalSetting.UserName);
        writer.AddSnippet(title, text);
    }

    public void SendWarningMessage(string userName, string authorName, string title, string text)
    {
        using var writer = new SlackWriter(this.slackEndponts, userName);
        writer.AddAttachment(new Attachment
        {
            AuthorName = authorName,
            Color = "#7CD197",
            Title = title,
            Text = text,
        });
    }
}
