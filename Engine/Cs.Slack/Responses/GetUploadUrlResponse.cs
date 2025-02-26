namespace Cs.Slack.Responses;

using System.Text.Json.Serialization;
using Cs.Slack.Abstracts;

public sealed class GetUploadUrlResponse : IResponse
{
    public bool Ok { get; set; }
    public string Error { get; set; } = string.Empty;

    [JsonPropertyName(name: "upload_url")]
    public string UploadUrl { get; set; } = string.Empty;
    [JsonPropertyName(name: "file_id")]
    public string FileId { get; set; } = string.Empty;
}