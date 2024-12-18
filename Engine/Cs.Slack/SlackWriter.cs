namespace Cs.Slack
{
    using System;
    using Cs.Slack.Abstracts;
    using Cs.Slack.Blocks;
    using Cs.Slack.Elements;

    public sealed class SlackWriter : MarkdownWriter, IDisposable
    {
        private readonly SlackMessageBuilder slackBuilder;

        public SlackWriter(string icon)
            : this(icon, title: string.Empty)
        {
        }

        public SlackWriter(string icon, string title)
        {
            this.slackBuilder = new SlackMessageBuilder(SlackGlobalSetting.Endpoints);
            this.slackBuilder.UserName = SlackGlobalSetting.UserName;
            this.slackBuilder.IconEmoji = icon;

            // note: title은 section을 추가하면 무시됩니다. section까지 안쓰고 싶은 간소한 버전일 때 사용합니다.
            if (string.IsNullOrEmpty(title) == false)
            {
                this.WriteBoldLine(title);
            }
        }

        public Attachment AddAttachment(string color, string title)
        {
            Attachment result = new()
            {
                Color = color,
                Blocks = new(),
            };

            if (string.IsNullOrEmpty(title) == false)
            {
                this.AddSection().AddText().WriteBoldLine(title);
            }

            this.slackBuilder.Attachments.Add(result);
            return result;
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
            if (string.IsNullOrEmpty(this.Text) == false)
            {
                this.slackBuilder.Text = this.Text;
            }

            this.slackBuilder.Dispose();
        }
    }
}
