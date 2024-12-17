namespace CutEditor.Services;

using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.ViewModel;

internal sealed class FileLoader(VmHome vmHome)
{
    public void Load()
    {
        var path = VmGlobalState.Instance.CutSceneDataFilePath;
        Log.Debug($"Loading files from {path}");

        var list = new List<CutScene>();
        var json = JsonUtil.Load(path);
        json.GetArray("Data", list, (e, i) => new CutScene(e));

        vmHome.AddCutScenes(list);

        Log.Info($"cutscene loading finished. {list.Count} cutscenes loaded.");
    }
}
