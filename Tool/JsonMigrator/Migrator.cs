namespace JsonMigrator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Cs.Core.Util;
    using Cs.Logging;
    using Cs.Perforce;

    using JsonMigrator.Config;

    using Microsoft.ClearScript.V8;
    using Newtonsoft.Json.Linq;

    public sealed class Migrator
    {
        private readonly List<MigrationStep> steps = new();
        private readonly List<string> targets = new();
        private string jsMain = string.Empty;

        private Migrator()
        {
        }

        public static Migrator? Create(MigrationConfig config)
        {
            var migrator = new Migrator();
            Dictionary<int, MigrationStep> loadSteps = new();
            foreach (var stepFile in Directory.EnumerateFiles(config.StepScriptPath))
            {
                var migration = MigrationStep.Create(stepFile);
                if (migration is null)
                {
                    ErrorContainer.Add($"invalid migration step:{stepFile}");
                    return null;
                }

                if (loadSteps.TryGetValue(migration.Version, out var exist))
                {
                    ErrorContainer.Add($"duplicated migration version:{migration.Version} file1:{exist.FilePath} file2:{stepFile}");
                    return null;
                }

                loadSteps.Add(migration.Version, migration);
            }

            migrator.steps.AddRange(loadSteps.Values.OrderBy(x => x.Version));

            foreach (var targetPath in config.TargetPaths)
            {
                var fileAttr = File.GetAttributes(targetPath);
                if (fileAttr.HasFlag(FileAttributes.Directory))
                {
                    foreach (var targetFile in Directory.EnumerateFiles(targetPath, "*.*", SearchOption.AllDirectories)
                        .Where(e => e.Contains("meta", StringComparison.InvariantCultureIgnoreCase) == false))
                    {
                        var fullPath = Path.GetFullPath(targetFile);

                        migrator.targets.Add(fullPath);
                    }
                }
                else
                {
                    if (Path.Exists(targetPath) == false)
                    {
                        ErrorContainer.Add($"target file not found. targetPath:{targetPath}");
                        continue;
                    }

                    var extension = Path.GetExtension(targetPath);
                    if (extension.Contains("meta", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var fullPath = Path.GetFullPath(targetPath);
                    migrator.targets.Add(fullPath);
                }
            }

            ErrorContainer.Validate();

            migrator.jsMain = File.ReadAllText("JsMain.js");

            return migrator;
        }

        public bool Migration(P4Commander p4Commander)
        {
            if (this.steps.Count == 0)
            {
                Log.Debug("no migration step");
                return true;
            }

            var v8Engine = new V8ScriptEngine();
            v8Engine.AddHostType("Console", typeof(Console)); // 디버그용
            v8Engine.Execute(this.jsMain);

            Log.Info($"Migration Start. #target:{this.targets.Count} #step:{this.steps.Count}");
            Parallel.ForEach(this.targets, target =>
            {
                var debugTarget = Path.GetFileName(target);
                var rowText = File.ReadAllText(target);
                var json = JObject.Parse(rowText);
                if (json is null)
                {
                    ErrorContainer.Add($"Migration target is not json format. target:{debugTarget}");
                    return;
                }

                var version = json.GetInt64("_Version", defValue: 0);
                if (this.steps.Any(e => e.Version > version) == false)
                {
                    return;
                }

                var jsonText = json.ToString();
                foreach (var step in this.steps.Where(e => e.Version > version))
                {
                    v8Engine.Execute(step.Script);
                    jsonText = v8Engine.Script.Run(jsonText, step.Version);
                }

                File.SetAttributes(target, FileAttributes.Normal);
                File.WriteAllText(target, jsonText);

                Log.Info($"Migration Complete. target:{debugTarget}");
                if (p4Commander.CheckIfOpened(target))
                {
                    return;
                }

                if (p4Commander.CheckIfChanged(target, out bool changed) == false)
                {
                    ErrorContainer.Add($"변경여부 확인 실패. fileName:{debugTarget}");
                    return;
                }

                if (changed == false)
                {
                    return;
                }

                if (p4Commander.OpenForEdit(target, out string p4Output) == false)
                {
                    ErrorContainer.Add($"p4 edit 실패. p4Output:{p4Output}");
                    return;
                }

                // For Debug
                //File.WriteAllText("C:\\Users\\rohjongwon\\Desktop\\TestJson\\migrated.json", jsonText);
                //break;
            });

            return true;
        }
    }
}
