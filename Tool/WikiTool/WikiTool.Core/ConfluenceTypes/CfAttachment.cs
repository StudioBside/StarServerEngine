namespace WikiTool.Core.ConfluenceTypes;

public sealed class CfAttachment
{
    public string Id { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public string MediaTypeDescription { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int FileSize { get; set; }
}
