namespace Cs.Slack.Blocks;

using Cs.Slack.Abstracts;
using Cs.Slack.Objects;

// https://api.slack.com/reference/block-kit/blocks#header
public sealed class Header : IBlock
{
    public string Type => "header";
    public TextObject Text { get; } = new TextObject { Type = "plain_text" };
}
