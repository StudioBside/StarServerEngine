namespace JsonMigrator.MigrationTargets;
using System.IO;
using Cs.Core.Perforce;
using Cs.Logging;
using JsonMigrator;
using Newtonsoft.Json.Linq;

internal sealed record class FileMigrationTarget(string Location, string Key, in P4Commander P4) : IMigrationTarget
{
    public JObject? Load()
    {
        var rowText = File.ReadAllText(this.Location);
        return JObject.Parse(rowText);
    }

    public void Save(string migratedData)
    {
        File.SetAttributes(this.Location, FileAttributes.Normal);
        File.WriteAllText(this.Location, migratedData);

        Log.Info($"Migration Complete. target:{this.Key}");

        if (this.P4.CheckIfOpened(this.Location))
        {
            return;
        }

        if (this.P4.CheckIfChangedNotDepotPath(this.Location, out bool changed) == false)
        {
            ErrorContainer.Add($"변경여부 확인 실패. fileName:{this.Key}");
            return;
        }

        if (changed == false)
        {
            return;
        }

        if (this.P4.OpenForEdit(this.Location, out string p4Output) == false)
        {
            ErrorContainer.Add($"p4 edit 실패. p4Output:{p4Output}");
            return;
        }
    }
}