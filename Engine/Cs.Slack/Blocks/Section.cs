namespace Cs.Slack.Blocks
{
    using System.Collections.Generic;
    using Cs.Slack.Abstracts;

    public sealed class Section : IBlock
    {
        public string Type => "section";
        public TextBlock? Text { get; private set; }
        public List<TextBlock> Fields { get; private set; } = new();

        public TextBlock AddText()
        {
            this.Text = new TextBlock();
            return this.Text;
        }

        public TextBlock AddField()
        {
            var result = new TextBlock();
            this.Fields.Add(result);
            return result;
        }
    }
}