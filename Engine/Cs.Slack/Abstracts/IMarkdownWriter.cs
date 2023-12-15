namespace Cs.Slack.Abstracts
{
    using System.Text;

    public abstract class MarkdownWriter
    {
        private readonly StringBuilder builder = new();

        public string Text => this.builder.ToString();

        public MarkdownWriter WriteBoldLine(string message)
        {
            this.builder.Append($"*{message}*\n");
            return this;
        }

        public MarkdownWriter WriteLine(string message)
        {
            this.builder.Append($"{message}\n");
            return this;
        }

        public MarkdownWriter WriteBold(string message)
        {
            this.builder.Append($"*{message}* ");
            return this;
        }

        public MarkdownWriter WriteList(string message)
        {
            this.builder.Append($":black_small_square: {message}\n");
            return this;
        }

        public MarkdownWriter Write(string message)
        {
            this.builder.Append($"{message} ");
            return this;
        }

        public override string ToString() => this.Text;
    }
}
