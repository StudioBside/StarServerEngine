namespace Cs.Slack.Objects;

using System.Text;

// https://api.slack.com/reference/block-kit/composition-objects#text
public sealed class TextObject
{
    private readonly StringBuilder builder = new();

    public string Type { get; set; } = "mrkdwn";
    public string Text => this.builder.ToString();

    public TextObject WriteBoldLine(string message)
    {
        this.builder.Append($"*{message}*\n");
        return this;
    }

    public TextObject WriteLine(string message)
    {
        this.builder.Append($"{message}\n");
        return this;
    }

    public TextObject WriteBold(string message)
    {
        this.builder.Append($"*{message}* ");
        return this;
    }

    public TextObject WriteList(string message)
    {
        this.builder.Append($":black_small_square: {message}\n");
        return this;
    }

    public TextObject Write(string message)
    {
        this.builder.Append($"{message} ");
        return this;
    }

    public override string ToString() => this.Text;
}
