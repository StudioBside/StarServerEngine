namespace Cs.Slack.Elements
{
    using System;
    using System.Collections.Generic;
    using Cs.Slack.Abstracts;
    using Cs.Slack.Blocks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public sealed class Attachment
    {
        [JsonProperty(PropertyName = "author_name")]
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public List<IBlock> Blocks { get; set; } = new List<IBlock>();

        public void AddDivider()
        {
            this.Blocks.Add(new Divider());
        }

        public Section AddSection()
        {
            var section = new Section();
            this.Blocks.Add(section);
            return section;
        }
    }
}
