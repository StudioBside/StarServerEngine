namespace WikiTool.Core;

public sealed class WjPage
{
    public int Id { get; init; }
    public string Path { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Content { get; init; }
    public string Render { get; init; }
    public string Toc { get; init; }
    public string ContentType { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public WjUser Author { get; init; }
    public WjUser Creator { get; init; }
}
