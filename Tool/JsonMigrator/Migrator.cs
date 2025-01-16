namespace JsonMigrator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cs.Core.Util;
using Cs.Logging;
using JsonMigrator.Config;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

internal sealed class Migrator
{
    private IScriptEngine jsEngine;

    private Migrator(IScriptEngine engine)
    {
        this.jsEngine = engine;
    }

    public static Migrator? Create(MigrationConfig config)
    {
        var jsMain = File.ReadAllText("JsMain.js");

        var v8Engine = new V8ScriptEngine();
        v8Engine.AddHostType("Console", typeof(Console)); // 디버그용
        v8Engine.Execute(jsMain);

        return new Migrator(v8Engine);
    }

    public bool Migration(MigrationStepContioner stepContainer, IReadOnlyList<IMigrationTarget> targets)
    {
        if (stepContainer.Count == 0)
        {
            Log.Debug("no migration step");
            return true;
        }

        Log.Info($"Migration Start. #target:{targets.Count} #step:{stepContainer.Count}");
        Parallel.ForEach(targets, target =>
        {
            var json = target.Load();
            if (json is null)
            {
                ErrorContainer.Add($"Migration target is not json format. target:{target.Key}");
                return;
            }

            var version = json.GetInt64("_Version", defValue: 0);
            if (stepContainer.Any(e => e.Version > version) == false)
            {
                return;
            }

            var jsonText = json.ToString();
            foreach (var step in stepContainer.Where(e => e.Version > version))
            {
                this.jsEngine.Execute(step.Script);
                jsonText = this.jsEngine.Script.Run(jsonText, step.Version);
            }

            target.Save(jsonText);

            // For Debug
            //File.WriteAllText("C:\\Users\\rohjongwon\\Desktop\\TestJson\\migrated.json", jsonText);
            //break;
        });

        return true;
    }
}
