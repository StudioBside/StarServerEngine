namespace Cs.Slack.Exceptions
{
    public sealed class ExceptionSlackSender : Cs.Exception.ISlackSender
    {
        private readonly SlackEndpoint[] slackEndponts;
       
        public ExceptionSlackSender(SlackEndpoint[] slackEndponts)
        {
            this.slackEndponts = slackEndponts;
        }

        public void SendSnippet(string title, string text)
        {
            using var builder = new SlackMessageBuilder(this.slackEndponts);
            builder.PresetSnippet(title, text);
        }

        public void SendMessage(string userName, string authorName, string title, string text)
        {
            using var builder = new SlackMessageBuilder(this.slackEndponts);
            builder.PresetAttachment(userName, authorName, title, text);
        }
    }
}
