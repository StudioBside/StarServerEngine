namespace JsonMigrator.MigrationTargets;
using Cs.Logging;
using JsonMigrator;
using Newtonsoft.Json.Linq;
using StringStorage.Detail;

internal sealed record LevelDbMigrationTarget(string Location, string Key, LevelDbController Controller) : IMigrationTarget
{
    public JObject? Load()
    {
        if (this.Controller.TryGet(this.Key, JObject.Parse, out JObject? result))
        {
            return result;
        }

        return null;
    }

    public void Save(string migratedData)
    {
        this.Controller.Upsert(this.Key, migratedData);
    }
}