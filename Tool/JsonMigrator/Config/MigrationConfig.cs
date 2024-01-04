namespace JsonMigrator.Config
{
    public sealed class MigrationConfig
    {
        public string TargetPath { get; set; } = string.Empty;
        public string StepScriptPath { get; set; } = string.Empty;
    }
}
