namespace JsonMigrator
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Cs.Core.Util;
    using Cs.Logging;
    using Cs.Perforce;

    using JsonMigrator.Config;

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

                if (string.IsNullOrEmpty(config.StepScriptPath) || string.IsNullOrEmpty(config.TargetPath))
                {
                    Log.Error("invalid config");
                    return -2;
                }

                Log.Debug($"stepScriptPath:{config.StepScriptPath}");
                Log.Debug($"TargetPath:{config.TargetPath}");
                Console.WriteLine();

                if (Directory.Exists(config.StepScriptPath) == false)
                {
                    Log.Info($"마이그레이션 스크립트 폴더가 없습니다. 성공으로 처리됩니다.");
                    return 0;
                }

                var stopwatch = Stopwatch.StartNew();
                if (P4Commander.TryCreate(out var p4Commander) == false)
                {
                    Log.Error($"p4 환경 정보 조회 실패");
                    return -3;
                }

                var targetFullPath = Path.GetFullPath(config.TargetPath);
                var migrator = Migrator.Create(config);
                if (migrator is null)
                {
                    return -4;
                }

                Log.DebugBold($"migration Ready. prepare:{stopwatch.Elapsed}");
                if (migrator.Migration(p4Commander) == false)
                {
                    return -5;
                }

                if (ErrorContainer.HasError)
                {
                    Log.Error($"Migration End. #Error:{ErrorContainer.ErrorCount}");
                    return -6;
                }

                p4Commander.RevertUnchnaged(targetFullPath, out _);
                Log.DebugBold($"migration End. elapsed:{stopwatch.Elapsed}");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }
    }
}