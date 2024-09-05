namespace JsonSchemaValidator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;

    using Cs.Logging;

    using Microsoft.CodeAnalysis.Sarif;
    using Microsoft.CodeAnalysis.Sarif.Writers;
    using Microsoft.Json.Schema;
    using Microsoft.Json.Schema.Validation;
    using static JsonSchemaValidator.Program;

    public sealed class SchemaValidator
    {
        private readonly Validator impl;
        private int errorCount;

        private SchemaValidator(string key, JsonSchema schema)
        {
            this.Key = key;
            this.impl = new Validator(schema);
        }

        public string Key { get; private set; }

        public static SchemaValidator? Create(Uri schemaLocation, string jsonPath)
        {
            string schemaText = string.Empty;
            if (schemaLocation.IsAbsoluteUri && schemaLocation.IsFile == false)
            {
                // http://, https://
                using (var httpClient = new HttpClient())
                {
                    schemaText = httpClient.GetStringAsync(schemaLocation).Result;
                }
            }
            else if (schemaLocation.IsAbsoluteUri && schemaLocation.IsFile)
            {
                // file://
                string schemaPath = Path.GetFullPath(schemaLocation.AbsolutePath);
                if (File.Exists(schemaPath) == false)
                {
                    return null;
                }

                schemaText = File.ReadAllText(schemaPath);
            }
            else
            {
                // ./, ../
                string basePath = Path.GetDirectoryName(jsonPath) ?? Environment.CurrentDirectory;
                string schemaPath = Path.GetFullPath(schemaLocation.OriginalString, basePath);
                if (File.Exists(schemaPath) == false)
                {
                    return null;
                }

                schemaText = File.ReadAllText(schemaPath);
            }

            if (string.IsNullOrEmpty(schemaText))
            {
                return null;
            }

            var schema = SchemaReader.ReadSchema(schemaText, schemaLocation.OriginalString);
            return new SchemaValidator(schemaLocation.GetFileName(), schema);
        }

        public void ReportSummery()
        {
            Log.Debug($"스키마 파일:{this.Key} #Error:{this.errorCount}");
        }

        internal void Validate(string jsonText, PathInfo jsonPath, SarifLogger logger)
        {
            var results = this.impl.Validate(jsonText, jsonPath.FullPath);
            if (results.Any())
            {
                this.errorCount += results.Length;

                Log.Error(Log.BuildHead(jsonPath.DebugName.ToString()));
                foreach (var result in results)
                {
                    ReportingDescriptor rule = RuleFactory.GetRuleFromRuleId(result.RuleId);
                    ErrorContainer.Add($"  {result.RuleId} {result.GetMessageText(rule)}");
                    logger.Log(rule, result, extensionIndex: null);
                }

                Console.WriteLine();
            }
        }
    }
}
