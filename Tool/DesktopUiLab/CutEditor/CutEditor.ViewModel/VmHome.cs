namespace CutEditor.ViewModel;

using CutEditor.Model;
using Du.Core.Bases;

public sealed class VmHome : VmPageBase
{
    private readonly List<CutScene> cutScenes = new();
    private CutScene? selectedCutScene;

    public VmHome()
    {
        this.Title = "VmHome";
    }

    public IList<CutScene> CutScenes => this.cutScenes;
    public CutScene? SelectedCutScene
    {
        get => this.selectedCutScene;
        set => this.SetProperty(ref this.selectedCutScene, value);
    }

    public void AddCutScenes(IEnumerable<CutScene> cutScenes)
    {
        this.cutScenes.Clear();
        this.cutScenes.AddRange(cutScenes);
    }

    //// --------------------------------------------------------------------------------------------
}
