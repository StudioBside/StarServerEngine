namespace Cs.Perforce;

public sealed class P4ConnectionInfo
{
    public string Uri { get; set; } = string.Empty;
    public string Workspace { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Pw { get; set; } = string.Empty;
}
