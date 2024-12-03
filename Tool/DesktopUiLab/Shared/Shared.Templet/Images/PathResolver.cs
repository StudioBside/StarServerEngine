namespace Shared.Templet.Images;

using System;
using System.Diagnostics.CodeAnalysis;
using Cs.Core;
using Microsoft.Extensions.Configuration;

public sealed class PathResolver
{
    private readonly Dictionary<string/*fileName*/, string/*fullPath*/> illustPaths = new(StringComparer.CurrentCultureIgnoreCase);
    private readonly Dictionary<string/*fileName*/, string/*fullPath*/> buffPaths = new(StringComparer.CurrentCultureIgnoreCase);

    public static PathResolver Instance => Singleton<PathResolver>.Instance;

    public void Initialize(IConfiguration config)
    {
        var illustRoot = config["IllustRoot"] ?? throw new Exception($"IllustRoot is not defined in the configuration file.");
        if (Directory.Exists(illustRoot))
        {
            foreach (var fileName in Directory.GetFiles(illustRoot, "*.png", SearchOption.AllDirectories))
            {
                this.illustPaths[Path.GetFileName(fileName)] = Path.GetFullPath(fileName);
            }
        }

        var buffIconRoot = config["BuffIconRoot"] ?? throw new Exception($"BuffIconRoot is not defined in the configuration file.");
        if (Directory.Exists(buffIconRoot))
        {
            foreach (var fileName in Directory.GetFiles(buffIconRoot, "*.png", SearchOption.AllDirectories))
            {
                this.buffPaths[Path.GetFileName(fileName)] = Path.GetFullPath(fileName);
            }
        }
    }

    public bool TryGetIllustPath(string fileName, [NotNullWhen(true)] out string? fullPath)
    {
        return this.illustPaths.TryGetValue(fileName, out fullPath);
    }

    public bool TryGetBuffPath(string fileName, [NotNullWhen(true)] out string? fullPath)
    {
        return this.buffPaths.TryGetValue(fileName, out fullPath);
    }
}
