namespace JsonMigrator;

using System.Collections.Generic;

internal interface IMigrationTargetFactory
{
    IEnumerable<IMigrationTarget> Create(string location);
    void Cleanup();
}