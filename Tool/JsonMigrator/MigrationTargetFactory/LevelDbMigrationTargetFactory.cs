namespace JsonMigrator.MigrationTargetFactory;

using System.Collections.Generic;
using System.IO;
using Cs.Core.Perforce;
using JsonMigrator.MigrationTargets;
using StringStorage.Detail;

internal sealed class LevelDbMigrationTargetFactory : IMigrationTargetFactory
{
    private readonly List<LevelDbController> controllers = new();
    private readonly P4Commander p4;

    public LevelDbMigrationTargetFactory(P4Commander p4)
    {
        this.p4 = p4;
    }

    public IEnumerable<IMigrationTarget> Create(string location)
    {
        var controller = new LevelDbController(location, this.p4);
        var fileName = Path.GetFileName(location);
        this.controllers.Add(controller);
        foreach (var value in controller)
        {
            yield return new LevelDbMigrationTarget(fileName, value.Key, controller);
        }
    }

    public void Cleanup()
    {
        foreach (var controller in this.controllers)
        {
            controller.Dispose();
        }
    }
}