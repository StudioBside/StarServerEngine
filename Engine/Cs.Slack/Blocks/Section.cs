namespace Cs.Slack.Blocks;

using System.Collections.Generic;
using Cs.Slack.Abstracts;
using Cs.Slack.Objects;

// https://api.slack.com/reference/block-kit/blocks#section
public sealed class Section : IBlock
{
    public string Type => "section";
    public TextObject? Text { get; private set; }
    public List<TextObject>? Fields { get; private set; }

    public TextObject AddTextObject()
    {
        this.Text = new TextObject();
        return this.Text;
    }

    public TextObject AddField()
    {
        if (this.Fields is null)
        {
            this.Fields = new List<TextObject>();
        }

        var result = new TextObject();
        this.Fields.Add(result);
        return result;
    }
}