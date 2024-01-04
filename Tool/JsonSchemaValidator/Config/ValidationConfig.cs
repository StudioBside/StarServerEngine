namespace JsonSchemaValidator.Config
{
    using System.Collections.Generic;

    public sealed class ValidationConfig
    {
        public List<string> TargetPaths { get; set; } = new();
        public string LogPath { get; set; } = string.Empty;
    }
}