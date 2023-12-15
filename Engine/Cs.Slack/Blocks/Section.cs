namespace Cs.Slack.Blocks
{
    using System.Collections.Generic;
    using Cs.Slack.Abstracts;

    public sealed class Section : IBlock
    {
        public string Type => "section";
        public TextBlock Text { get; private set; }
        public List<TextBlock> Fields { get; private set; }

        public TextBlock AddText()
        {
            this.Text = new TextBlock();
            return this.Text;
        }

        public TextBlock AddField()
        {
            if (this.Fields == null)
            {
                this.Fields = new();
            }

            var result = new TextBlock();
            this.Fields.Add(result);
            return result;
        }
    }
}
