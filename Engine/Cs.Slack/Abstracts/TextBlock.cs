namespace Cs.Slack.Blocks
{
    using Cs.Slack.Abstracts;

    public sealed class TextBlock : MarkdownWriter
    {
        public string Type { get; set; } = "mrkdwn";
    }
}
