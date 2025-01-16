namespace JsonMigrator.MigrationTargetFactory;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cs.Core.Perforce;
using Cs.Logging;
using JsonMigrator.MigrationTargets;

internal sealed class FileMigrationTargetFactory : IMigrationTargetFactory
{
    private readonly List<string> rootPaths = new();
    private readonly P4Commander p4;

    public FileMigrationTargetFactory(P4Commander p4)
    {
        this.p4 = p4;
    }

    public IEnumerable<IMigrationTarget> Create(string location)
    {
        if (Path.Exists(location) == false)
        {
            ErrorContainer.Add($"target file not found. location:{location}");
            yield break;
        }

        var fullPath = Path.GetFullPath(location);
        this.rootPaths.Add(fullPath);

        var fileAttr = File.GetAttributes(fullPath);
        if (fileAttr.HasFlag(FileAttributes.Directory))
        {
            foreach (var targetFile in Directory.EnumerateFiles(fullPath, "*.*", SearchOption.AllDirectories)
                .Where(e => e.Contains("meta", StringComparison.InvariantCultureIgnoreCase) == false))
            {
                var filePath = Path.GetFullPath(targetFile);
                var debugName = Path.GetFileName(filePath);
                yield return new FileMigrationTarget(filePath, debugName, this.p4);
            }
        }
        else
        {
            var extension = Path.GetExtension(fullPath);
            if (extension.Contains("meta", StringComparison.InvariantCultureIgnoreCase))
            {
                yield break;
            }

            var debugName = Path.GetFileName(fullPath);
            yield return new FileMigrationTarget(fullPath, debugName, this.p4);
        }
    }

    public void Cleanup()
    {
        foreach (var path in this.rootPaths)
        {
            this.p4.RevertUnchnaged(path, out _);
        }
    }
}