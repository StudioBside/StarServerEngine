namespace JsonSchemaValidator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Cs.Core.Util;
    using Cs.Logging;

    using JsonSchemaValidator.Config;

    using Microsoft.CodeAnalysis.Sarif;
    using Microsoft.CodeAnalysis.Sarif.Writers;
    using Microsoft.Json.Schema;
    using Microsoft.Json.Schema.Validation;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal static class Program
    {
        internal static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string configFileName = "config.validation.json";
            if (args.Length > 0)
            {
                configFileName = args[0];
            }

            ValidationConfig config = null!;
            try
            {
                config = JsonUtil.Load<ConfigHolder>(configFileName).Validation;

                if (config is null || config.TargetPaths.Count <= 0)
                {
                    Log.Error("invalid config");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"load config fail. {ex.Message}");
                return -1;
            }
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            var targetFiles = GatherTargetFiles(config);

            Log.Info($"검사 대상 파일 {targetFiles.Count()}개 발견. elapsed:{stopwatch.Elapsed}");
            stopwatch.Restart();

            using SarifLogger logger = CreateLogger(config.LogPath, targetFiles.Select(e => e.FullPath));

            Dictionary<string, SchemaValidator> validators = new();
            foreach (var fileInfo in targetFiles)
            {
                try
                {
                    string jsonText = File.ReadAllText(fileInfo.FullPath);
                    var json = JsonConvert.DeserializeObject<JToken>(jsonText);
                    if (json is null)
                    {
                        ErrorContainer.Add($"json load fail. path:{fileInfo.DebugName}");
                        continue;
                    }

                    if (json is JArray)
                    {
                        ErrorContainer.Add($"json is JArray, not found schema. path:{fileInfo.DebugName}");
                        continue;
                    }

                    var schemaPath = json.GetString("$schema");
                    if (string.IsNullOrEmpty(schemaPath))
                    {
                        ErrorContainer.Add($"schema not set. path:{fileInfo.DebugName}");
                        continue;
                    }

                    if (Uri.TryCreate(schemaPath, UriKind.RelativeOrAbsolute, out var schemaLocation) == false)
                    {
                        ErrorContainer.Add($"$schema must be a Uri. path:{fileInfo} schemaPath:{schemaPath}");
                        continue;
                    }

                    if (validators.TryGetValue(schemaLocation.GetFileName(), out var validator) == false)
                    {
                        validator = SchemaValidator.Create(schemaLocation, fileInfo.FullPath);
                        if (validator is null)
                        {
                            ErrorContainer.Add($"schema file not found. jsonPath:{fileInfo} schemaPath:{schemaPath} originalString:{schemaLocation.OriginalString}");
                            continue;
                        }

                        validators.Add(schemaLocation.GetFileName(), validator);
                    }

                    validator.Validate(json.ToString(), fileInfo, logger);
                }
                catch (JsonSyntaxException ex)
                {
                    ReportResult(ex.ToSarifResult());
                }
                catch (SchemaValidationException ex)
                {
                    foreach (SchemaValidationException wrappedException in ex.WrappedExceptions)
                    {
                        Result result = ResultFactory.CreateResult(wrappedException.JToken, wrappedException.ErrorNumber, wrappedException.Args);
                        foreach (var location in result.Locations)
                        {
                            location.PhysicalLocation.ArtifactLocation = new ArtifactLocation
                            {
                                Uri = new Uri(fileInfo.FullPath, UriKind.RelativeOrAbsolute),
                            };
                        }

                        ReportResult(result);
                    }
                }
                catch (Exception ex)
                {
                    ErrorContainer.Add($"""
                                        ------------------------------------------------------------
                                        Exception Path:{fileInfo}
                                        Message:{ex.Message}
                                        StackTrace:{ex.StackTrace}
                                        ------------------------------------------------------------
                                        """);
                }
            }

            Log.WriteFileLine = false;
            Log.Debug("------------------------------------------------------------");
            Log.Debug($"스키마 검사 완료. elapsed:{stopwatch.Elapsed}");
            foreach (var validator in validators.Values)
            {
                validator.ReportSummery();
            }

            return ErrorContainer.HasError ? -2 : 0;
        }

        private static IEnumerable<PathInfo> GatherTargetFiles(ValidationConfig config)
        {
            foreach (var path in config.TargetPaths)
            {
                var attribute = File.GetAttributes(path);
                if (attribute.HasFlag(FileAttributes.Directory))
                {
                    var targetDirectory = Path.GetFullPath(path);
                    if (Directory.Exists(targetDirectory) == false)
                    {
                        Log.ErrorAndExit($"target directory not found. path:{targetDirectory}");
                        yield break;
                    }

                    foreach (var targetPath in Directory.EnumerateFiles(targetDirectory, "*.*", SearchOption.AllDirectories)
                        .Where(e => e.Contains("json", StringComparison.InvariantCultureIgnoreCase) && e.Contains("meta") == false)
                        .Select(e => new PathInfo(e, targetDirectory.Length)))
                    {
                        yield return targetPath;
                    }
                }
                else
                {
                    var targetPath = Path.GetFullPath(path);
                    yield return new PathInfo(targetPath, debugIndex: 0);
                }
            }
        }

        private static void ReportResult(Result result)
        {
            ReportingDescriptor rule = RuleFactory.GetRuleFromRuleId(result.RuleId);

            ErrorContainer.Add(result.FormatForVisualStudio(rule));
        }

        private static SarifLogger CreateLogger(string logPath, IEnumerable<string> analysisTargets)
        {
            if (string.IsNullOrEmpty(logPath))
            {
                logPath = "../../Log/JsonSchemaValidator.sarif";
            }

            var directory = Path.GetDirectoryName(logPath);
            if (directory is null)
            {
                logPath = "./JsonSchemaValidator.sarif";
            }
            else
            {
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
            }

            return new SarifLogger(
                outputFilePath: logPath,
                analysisTargets: analysisTargets,
                kinds: new ResultKindSet
                {
                    ResultKind.Fail,
                    ResultKind.Informational,
                    ResultKind.None,
                    ResultKind.NotApplicable,
                    ResultKind.Open,
                    ResultKind.Pass,
                    ResultKind.Review,
                },
                levels: new FailureLevelSet
                {
                    FailureLevel.Error,
                    FailureLevel.None,
                    FailureLevel.Note,
                    FailureLevel.Warning,
                },
                invocationTokensToRedact: null);
        }

        internal record struct PathInfo
        {
            private readonly int debugIndex;
            public PathInfo(string originalString, int debugIndex)
            {
                this.FullPath = Path.GetFullPath(originalString);
                this.debugIndex = debugIndex;
            }

            public string FullPath { get; }

            public ReadOnlySpan<char> DebugName => this.FullPath.AsSpan(this.debugIndex);
        }
    }
}