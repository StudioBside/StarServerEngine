namespace Shared.Templet.UnitScripts;

using System;
using Cs.Core;

public sealed class UnitScriptContainer
{
    private readonly Dictionary<string, UnitScript> scripts = new();

    public static UnitScriptContainer Instance => Singleton<UnitScriptContainer>.Instance;
    public IEnumerable<UnitScript> Values => this.scripts.Values;

    public void Load(string rootPath)
    {
        foreach (var file in Directory.GetFiles(rootPath, "*.txt", SearchOption.AllDirectories))
        {
            var script = new UnitScript(file);
            this.scripts.Add(script.FileName, script);
        }
    }

    public UnitScript? Find(string fileName)
    {
        this.scripts.TryGetValue(fileName, out var script);
        return script;
    }
}
