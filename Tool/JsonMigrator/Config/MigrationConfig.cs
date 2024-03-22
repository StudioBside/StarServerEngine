namespace JsonMigrator.Config
{
    using System;

    public sealed class MigrationConfig
    {
        public string[] TargetPaths { get; set; } = Array.Empty<string>();
        public string StepScriptPath { get; set; } = string.Empty;
    }
}
