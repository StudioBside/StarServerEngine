namespace WikiTool.Core;

using Cs.Logging;

public sealed class WjPage
{
    private readonly List<string> pathTokens = new();

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
    public IEnumerable<string> GetPathTokens => this.pathTokens;

    public void BuildPath(HashSet<string> pathSet)
    {
        this.pathTokens.Clear();
        var tokens = this.Path.Split('/');

        // except last one
        foreach (var token in tokens[..^1])
        {
            var candidate = token;
            int retryCount = 0;
            
            while (pathSet.Add(candidate) == false)
            {
                candidate = $"{token} ({++retryCount})";
            }

            this.pathTokens.Add(token);
            if (retryCount > 0)
            {
                Log.Warn($"Duplicated path: {token} -> {candidate}");
            }
        }
    }
}
