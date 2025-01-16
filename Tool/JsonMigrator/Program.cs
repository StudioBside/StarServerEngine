namespace JsonMigrator;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cs.Core.Perforce;
using Cs.Core.Util;
using Cs.Logging;

using JsonMigrator.Config;
using JsonMigrator.MigrationTargetFactory;
using static JsonMigrator.Config.MigrationConfig;

internal class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string configFileName = "config.migration.json";
        if (args.Length > 0)
        {
            configFileName = args[0];
        }

        Log.Debug($"loading config file:{configFileName}");

        try
        {
            var config = JsonUtil.Load<ConfigHolder>(configFileName).Migration;

            if (string.IsNullOrEmpty(config.StepScriptPath) || config.TargetLocations.Length <= 0)
            {
                Log.Error("invalid config");
                return -2;
            }

            Log.Debug($"stepScriptPath:{config.StepScriptPath}");
            Log.Info(string.Empty);

            var stepContainer = MigrationStepContioner.Create(config.StepScriptPath);
            if (stepContainer is null)
            {
                return -3;
            }

            // 마이그레이션할 정보가 없음
            if (stepContainer.Count <= 0)
            {
                return 0;
            }

            if (P4Commander.TryCreate(out var p4Commander) == false)
            {
                Log.Error($"p4 can not found information");
                return -3;
            }

            IMigrationTargetFactory? factory = config.LocationType switch
            {
                MigrationLocationType.File => new FileMigrationTargetFactory(p4Commander),
                MigrationLocationType.LevelDb => new LevelDbMigrationTargetFactory(p4Commander),
                _ => null,
            };

            if (factory is null)
            {
                var validTypes = string.Join(", ", EnumUtil<MigrationLocationType>.GetValues());
                Log.Error($"invalid location type:{config.LocationType}");
                Log.Info($"valid types:{validTypes}");
                return -4;
            }

            var stopwatch = Stopwatch.StartNew();
            var migrator = Migrator.Create(config);
            if (migrator is null)
            {
                return -5;
            }

            List<IMigrationTarget> targetList = new();
            foreach (var targetLocation in config.TargetLocations)
            {
                targetList.AddRange(factory.Create(targetLocation));
            }

            Log.DebugBold($"migration Ready. prepare:{stopwatch.Elapsed}");
            if (migrator.Migration(stepContainer, targetList) == false)
            {
                return -6;
            }

            if (ErrorContainer.HasError)
            {
                Log.Error($"Migration End. #Error:{ErrorContainer.ErrorCount}");
                return -7;
            }

            factory.Cleanup();
            Log.DebugBold($"migration End. elapsed:{stopwatch.Elapsed}");
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return -1;
        }

        return 0;
    }
}