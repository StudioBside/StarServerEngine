namespace WikiTool.Core;

using System.Text;
using Cs.Logging;

internal readonly struct UniquePathBuilder
{
    private readonly PathNode root;
    private readonly HashSet<string> pageNames = new();
    
    public UniquePathBuilder()
    {
        this.root = new PathNode("root", "root", null, this.pageNames);
    }

    public List<string> Calculate(string originPath)
    {
        var pathTokens = originPath.Split('/');
        var node = this.root;
        for (int i = 0; i < pathTokens.Length - 1; i++)
        {
            var pathToken = pathTokens[i];
            var child = node.Children.FirstOrDefault(e => e.OriginName == pathToken);
            if (child is null)
            {
                var uniqueName = pathToken;
                int retryCount = 0;
                while (this.pageNames.Add(uniqueName) == false)
                {
                    uniqueName = $"{pathToken} ({++retryCount})";
                }

                child = new PathNode(pathToken, uniqueName, node, this.pageNames);
                node.Children.Add(child);

                if (retryCount > 0)
                {
                    Log.Warn($"Duplicated path: {child.BuildOriginPath()} -> {child.BuildUniquePath()}");
                }
            }

            node = child;
        }

        return node.MakeTokens();
    }
    
    private sealed class PathNode
    {
        public PathNode(string name, string uniqueName, PathNode? parent, HashSet<string> pageNames)
        {
            this.OriginName = name;
            this.UniqueName = uniqueName;
            this.Parent = parent;
        }

        public bool IsRoot => this.Parent is null;
        public string OriginName { get; }
        public string UniqueName { get; }
        public PathNode? Parent { get; }
        public List<PathNode> Children { get; } = new();

        public List<string> MakeTokens()
        {
            var tokens = new List<string>();
            var node = this;
            while (node is not null && node.IsRoot == false)
            {
                tokens.Add(node.UniqueName);
                node = node.Parent;
            }

            tokens.Reverse();
            return tokens;
        }

        public string BuildOriginPath()
        {
            var sb = new StringBuilder();
            var node = this;
            while (node is not null && node.IsRoot == false)
            {
                sb.Insert(0, $"/{node.OriginName}");
                node = node.Parent;
            }

            return sb.ToString();
        }

        public string BuildUniquePath()
        {
            var sb = new StringBuilder();
            var node = this;
            while (node is not null && node.IsRoot == false)
            {
                sb.Insert(0, $"/{node.UniqueName}");
                node = node.Parent;
            }

            return sb.ToString();
        }
    }
}
