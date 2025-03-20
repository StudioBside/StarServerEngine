namespace Shared.Templet.UnitScripts;

using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.TempletTypes;

public sealed class UnitScript : ISearchable
{
    private readonly List<Unit> references = new();
    private readonly List<BattleState> battleStates = new();
    private readonly List<UiState> uiStates = new();

    public UnitScript(string path)
    {
        this.FullPath = Path.GetFullPath(path);
        this.FileName = Path.GetFileName(path);
        this.FullText = File.ReadAllText(path);
        this.Loc = this.FullText.Count(c => c == '\n');

        var token = JObject.Parse(this.FullText)["Data"];
        if (token is null)
        {
            Log.Error($"UnitScript: {this.FileName} is not a valid UnitScript file.");
            return;
        }

        token.TryGetArray("BattleStates", this.battleStates, (e, i) => new BattleState(e));
        token.TryGetArray("UiStates", this.uiStates, (e, i) => new UiState(e));
    }

    public static IEnumerable<UnitScript> Values => UnitScriptContainer.Instance.Values;
    public IReadOnlyList<Unit> References => this.references;
    public IReadOnlyList<BattleState> BattleStates => this.battleStates;
    public IReadOnlyList<UiState> UiStates => this.uiStates;
    public string FileName { get; }
    public string FullPath { get; }
    public string FullText { get; }
    public int Loc { get; }

    public bool IsTarget(string keyword)
    {
        return this.FileName.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    internal void AddReference(Unit unit)
    {
        this.references.Add(unit);
    }
}
