namespace JsonMigrator;

using Newtonsoft.Json.Linq;

internal interface IMigrationTarget
{
    string Location { get; }
    string Key { get; }

    JObject? Load();
    void Save(string migratedData);
}