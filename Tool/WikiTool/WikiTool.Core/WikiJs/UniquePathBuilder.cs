namespace WikiTool.Core;

using Cs.Logging;

internal readonly struct UniquePathBuilder
{
    private readonly PathNode root;
    private readonly HashSet<string> pageNames = new();
    private readonly Dictionary<string /*orginPath*/, IReadOnlyList<string> /*transPath*/> finalPaths = new();
    
    public UniquePathBuilder()
    {
        this.root = new PathNode("root", null, this.pageNames);
    }
    
    private sealed class PathNode
    {
        public PathNode(string name, PathNode? parent, HashSet<string> pageNames)
        {
            this.OriginName = name;
            this.Parent = parent;
            
            int retryCount = 0;
            var candidate = name;
            while (pageNames.Add(candidate) == false)
            {
                candidate = $"{name} ({++retryCount})";
            }
            
            this.UniqueName = candidate;
            if (retryCount > 0)
            {
                Log.Warn($"Duplicated path: {name} -> {candidate}");
            }
        }

        public string OriginName { get; }
        public string UniqueName { get; }
        public PathNode? Parent { get; }
        public List<PathNode> Children { get; } = new();
    }
}
