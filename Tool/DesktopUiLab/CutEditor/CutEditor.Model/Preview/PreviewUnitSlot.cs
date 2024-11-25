namespace CutEditor.Model.Preview;

using CommunityToolkit.Mvvm.ComponentModel;
using NKM;
using Shared.Templet.TempletTypes;

public sealed class PreviewUnitSlot : ObservableObject
{
    private Unit? unit;

    public PreviewUnitSlot(CutsceneUnitPos cutsceneUnitPos)
    {
        this.Position = cutsceneUnitPos;
    }

    public CutsceneUnitPos Position { get; }
    public int VisualNumber => (int)this.Position;

    public Unit? Unit
    {
        get => this.unit;
        set => this.SetProperty(ref this.unit, value);
    }

    public override string ToString()
    {
        return $"[{this.VisualNumber}] {this.Unit?.Name}";
    }
}
