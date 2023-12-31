namespace Excel2Json.Functions
{
    using Cs.Logging;

    internal static class FileSystem
    {
        public static void GuaranteePath(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath == null)
            {
                Log.Error($"[FileSystem] invalid filePath:{filePath}");
                return;
            }

            if (Directory.Exists(directoryPath))
            {
                return;
            }

            Log.Debug($"[FileSystem] create target path:{directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }
    }
}
