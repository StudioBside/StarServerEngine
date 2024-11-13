namespace CutEditor.ViewModel.Detail;

using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;

internal sealed class CutFileIo
{
    public static string GetTextFileName(string cutsceneName)
    {
        return Path.Combine(VmGlobalState.Instance.TextFilePath, $"CLIENT_{cutsceneName}.exported");
    }

    public static IEnumerable<Cut> LoadCutData(string cutsceneName)
    {
        var textFileName = GetTextFileName(cutsceneName);
        if (File.Exists(textFileName) == false)
        {
            Log.Debug($"cutscene file not found: {textFileName}");
            return Enumerable.Empty<Cut>();
        }

        var result = new List<Cut>();
        var json = JsonUtil.Load(textFileName);
        json.GetArray("Data", result, (e, i) => new Cut(e));

        return result;
    }
}
