namespace Cs.Core.Util
{
    using System.IO;

    public static class FileUtil
    {
        public static bool GetFileSize(string filePath, out long fileSize)
        {
            if (File.Exists(filePath) == false)
            {
                fileSize = 0;
                return false;
            }

            fileSize = new FileInfo(filePath).Length;
            return true;
        }
    }
}
