namespace Cs.Slack.Blocks
{
    using Cs.Slack.Abstracts;

    public sealed class Header : IBlock
    {
        public string Type => "header";
        public TextBlock Text { get; } = new TextBlock { Type = "plain_text" };
    }
}
