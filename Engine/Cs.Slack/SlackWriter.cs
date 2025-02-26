namespace Cs.Slack;

using System;
using Cs.Slack.Abstracts;
using Cs.Slack.Blocks;
using Cs.Slack.Elements;

public sealed class SlackWriter : IDisposable
{
    private readonly SlackMessageBuilder slackBuilder;

    public SlackWriter(string icon)
    {
        this.slackBuilder = new SlackMessageBuilder(SlackGlobalSetting.Endpoints);
        this.slackBuilder.UserName = SlackGlobalSetting.UserName;
        this.slackBuilder.IconEmoji = icon;
    }

    public SlackWriter(SlackEndpoint[] enspoints, string userName)
    {
        this.slackBuilder = new SlackMessageBuilder(enspoints);
        this.slackBuilder.UserName = userName;
    }

    public void AddAttachment(Attachment attachment)
    {
        this.slackBuilder.Attachments.Add(attachment);
    }

    public void AddSnippet(string title, string text)
    {
        this.slackBuilder.Snippet = new SnippetData(title, text);
    }

    public void AddHeader(string text)
    {
        var header = new Header();
        header.Text.Write(text);
        this.slackBuilder.Blocks.Add(header);
    }

    public void AddDivider()
    {
        this.slackBuilder.Blocks.Add(new Divider());
    }

    public Section AddSection()
    {
        var section = new Section();
        this.slackBuilder.Blocks.Add(section);
        return section;
    }

    public void CancelSendMessage()
    {
        this.slackBuilder.CancelSendMessage();
    }

    public void Dispose()
    {
        this.slackBuilder.Dispose();
    }
}
