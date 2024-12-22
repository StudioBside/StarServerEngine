namespace CutEditor.Model;

using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;

public sealed class CutSceneContainer
{
    private readonly Dictionary<int, CutScene> cutScenes = new();
    private readonly Dictionary<string, CutScene> byNames = new();

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
            this.byNames.Add(cutScene.FileName, cutScene);
        }

        Log.Info($"cutscene loading finished. {list.Count} cutscenes loaded.");
    }

    public CutScene? Find(string fileName)
    {
        return this.byNames.TryGetValue(fileName, out var cutScene) ? cutScene : null;
    }

    public CutScene? Find(int id)
    {
        return this.cutScenes.TryGetValue(id, out var cutScene) ? cutScene : null;
    }
}
