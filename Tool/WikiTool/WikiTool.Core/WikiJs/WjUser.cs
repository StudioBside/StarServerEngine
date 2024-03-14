namespace WikiTool.Core;

public sealed class WjUser
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}
