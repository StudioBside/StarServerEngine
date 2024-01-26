namespace SlackAssist.Fremawork.Slack;

using System;
using System.IO;
using System.Text;
using SlackNet.Blocks;

internal sealed class MarkdownBuilder : IDisposable
{
    private readonly StringBuilder stringBuilder = new();
    private readonly StringWriter writer;

    public MarkdownBuilder()
    {
        this.writer = new StringWriter(this.stringBuilder);
    }

    public void Write(string text)
    {
        this.writer.Write(text);
    }

    public void WriteBold(string text)
    {
        this.writer.Write($"*{text}*");
    }

    public void WriteBoldLine(string text)
    {
        this.writer.WriteLine($"*{text}*");
    }

    public void WriteLine(string text)
    {
        this.writer.WriteLine(text);
    }

    public void WriteLine()
    {
        this.writer.WriteLine();
    }

    public TextObject FlushToTextObject()
    {
        var result = new Markdown { Text = this.stringBuilder.ToString() };
        this.stringBuilder.Clear();
        return result;
    }

    public SectionBlock FlushToSectionBlock()
    {
        var section = new SectionBlock
        {
            Text = new Markdown { Text = this.stringBuilder.ToString() },
        };

        this.stringBuilder.Clear();
        return section;
    }

    public void Dispose()
    {
        this.writer.Dispose();
    }
}
