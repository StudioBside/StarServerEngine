namespace JsonMigrator
{
    using System.IO;

    public class MigrationStep
    {
        private MigrationStep(int version, string filePath, string script)
        {
            this.Version = version;
            this.FilePath = filePath;
            this.Script = script;
        }

        public int Version { get; }
        public string FilePath { get; }
        public string Script { get; }

        public static MigrationStep? Create(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            if (int.TryParse(fileName, out var version) == false)
            {
                return null;
            }

            var fullPath = Path.GetFullPath(path);
            var script = File.ReadAllText(fullPath);
            return new MigrationStep(version, fullPath, script);
        }
    }
}
