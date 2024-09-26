namespace CutEditor.ViewModel;

using CutEditor.Model;
using Du.Core.Bases;

public sealed class VmHome : VmPageBase
{
    private readonly List<CutScene> cutScenes = new();
    public VmHome()
    {
        this.Title = "VmHome";
    }

    public void AddCutScenes(IEnumerable<CutScene> cutScenes)
    {
        this.cutScenes.Clear();
        this.cutScenes.AddRange(cutScenes);
    }

    //// --------------------------------------------------------------------------------------------
}
