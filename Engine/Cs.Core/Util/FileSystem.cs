namespace Cs.Core.Util;

using System.IO;
using Cs.Logging;

public static class FileSystem
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

    public static bool SafeDelete(string filePath)
    {
        if (File.Exists(filePath) == false)
        {
            // 파일이 없는 경우도 성공으로 간주한다.
            return true;
        }

        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (IOException e)
        {
            Log.Error($"[FileSystem] failed to delete file. path:{filePath} e:{e}");
            return false;
        }
    }
}
