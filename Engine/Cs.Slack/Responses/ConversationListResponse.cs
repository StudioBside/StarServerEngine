namespace Cs.Slack.Responses
{
    using System.Collections.Generic;
    using Cs.Slack.Abstracts;
    using Newtonsoft.Json;

    public sealed class ConversationListResponse : IResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; } = string.Empty;

        public List<Channel> Channels { get; set; } = new();

        [JsonProperty(PropertyName = "response_metadata")]
        public ResponseMetadata? Metadata { get; set; }

        // note : 필요한 속성은 필요할때마다 추가합니다.
        public class Channel
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public class ResponseMetadata
        {
            [JsonProperty(PropertyName = "next_cursor")]
            public string NextCursor { get; set; } = string.Empty;
        }
    }
}