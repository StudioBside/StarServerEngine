namespace Shared.Templet.Images;

using System;
using System.Diagnostics.CodeAnalysis;
using Cs.Core;

public sealed class PathResolver
{
    private readonly Dictionary<string/*fileName*/, string/*fullPath*/> illustPaths = new();

    public static PathResolver Instance => Singleton<PathResolver>.Instance;

    public void SetIllustRoot(string illustRoot)
    {
        foreach (var fileName in Directory.GetFiles(illustRoot, "*.png", SearchOption.AllDirectories))
        {
            this.illustPaths[Path.GetFileName(fileName)] = Path.GetFullPath(fileName);
        }
    }

    public bool TryGetIllustPath(string fileName, [NotNullWhen(true)] out string? fullPath)
    {
        return this.illustPaths.TryGetValue(fileName, out fullPath);
    }
}
