namespace WikiTool.Core.WikiJs;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cs.Logging;

internal sealed class AssetController
{
    private readonly HashSet<string> files = new();
    private string assetPath = string.Empty;

    public void Initialize(string assetPath)
    {
        if (Directory.Exists(assetPath) == false)
        {
            Log.Error($"Asset path not found: {assetPath}");
            return;
        }

        this.assetPath = assetPath;
        var files = Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var transformed = file
                .Replace(assetPath, string.Empty) // 상대경로
                .Replace('\\', '/'); // 경로 구분자 통일

            this.files.Add(transformed);
        }
  
        Log.Info($"Loaded {this.files.Count} assets.");
    }

    public bool Contains(string path, [MaybeNullWhen(false)] out string fullPath)
    {
        if (this.files.Contains(path) == false)
        {
            fullPath = null;
            return false;
        }

        if (path.StartsWith('/'))
        {
            path = path[1..];
        }

        fullPath = Path.Combine(this.assetPath, path);
        fullPath = Path.GetFullPath(fullPath);
        return true;
    }
}
