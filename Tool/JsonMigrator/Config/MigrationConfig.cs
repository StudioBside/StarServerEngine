namespace JsonMigrator.Config
{
    using System;

    public sealed class MigrationConfig
    {
        public enum MigrationLocationType
        {
            File,
            LevelDb,
        }

        public MigrationLocationType LocationType { get; set; }
        public string[] TargetLocations { get; set; } = Array.Empty<string>();
        public string StepScriptPath { get; set; } = string.Empty;
    }
}