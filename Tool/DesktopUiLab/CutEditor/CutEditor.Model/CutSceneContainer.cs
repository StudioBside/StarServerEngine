namespace CutEditor.Model;

using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;

public sealed class CutSceneContainer
{
    private readonly Dictionary<int, CutScene> cutScenes = new();

    public static CutSceneContainer Instance => Singleton<CutSceneContainer>.Instance;
    public IEnumerable<CutScene> CutScenes => this.cutScenes.Values;

    public void Load(string path)
    {
        Log.Debug($"Loading files from {path}");

        var list = new List<CutScene>();
        var json = JsonUtil.Load(path);
        json.GetArray("Data", list, (e, i) => new CutScene(e));

        foreach (var cutScene in list)
        {
            this.cutScenes.Add(cutScene.Id, cutScene);
        }

        Log.Info($"cutscene loading finished. {list.Count} cutscenes loaded.");
    }
}
