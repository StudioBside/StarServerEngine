﻿namespace WikiTool.Core;

using Cs.Logging;

public sealed class WjPage
{
    public int Id { get; init; }
    public required string Path { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Content { get; init; }
    public required string Render { get; init; }
    public required string Toc { get; init; }
    public required string ContentType { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public required WjUser Author { get; init; }
    public required WjUser Creator { get; init; }

    public string UniqueTitle => $"{this.Title} ({this.Id})";
    public IEnumerable<string> PathTokens { get; set; } = null!;
}
