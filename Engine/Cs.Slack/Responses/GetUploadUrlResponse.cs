namespace Cs.Slack.Responses
{
    using Cs.Slack.Abstracts;
    using Newtonsoft.Json;

    public sealed class GetUploadUrlResponse : IResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "upload_url")]
        public string UploadUrl { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "file_id")]
        public string FileId { get; set; } = string.Empty;
    }
}