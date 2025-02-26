namespace Cs.Slack.Blocks;

using Cs.Slack.Abstracts;

// https://api.slack.com/reference/block-kit/blocks#divider
internal sealed class Divider : IBlock
{
    public string Type => "divider";
}
