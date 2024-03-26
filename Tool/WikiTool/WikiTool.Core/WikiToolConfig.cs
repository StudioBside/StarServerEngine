namespace WikiTool.Core;

public sealed record WikiToolConfig
{
    public required ConfluenceConfig Confluence { get; init; }
    public required string WikiJsBackupPath { get; init; }
    public required List<string> BatchCommands { get; init; }

    public sealed record ConfluenceConfig
    {
        public required string Url { get; init; } 
        public required string Username { get; init; }
        public required string Password { get; init; }
    }
}
