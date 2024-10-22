namespace CutEditor.ViewModel;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Cs.Core;
using Cs.Core.Util;
using CutEditor.ViewModel.Detail;

public sealed class GlobalState
{
    private VmCuts.CrateParam? vmCutsCreateParam;

    public static GlobalState Instance => Singleton<GlobalState>.Instance;

    internal VmCuts.CrateParam? VmCutCreateParam { get; set; }

    public void Initialize()
    {
        var templateSource = Assembly.GetExecutingAssembly().GetResourceString("CutEditor.ViewModel.TextTemplates.CutsOutput.stg");
        StringTemplateFactory.Instance.CreateFromString("CutsOutput", templateSource);
    }

    internal void ReserveVmCuts(VmCuts.CrateParam param)
    {
        this.vmCutsCreateParam = param;
    }

    internal bool PopVmCuts([MaybeNullWhen(false)] out VmCuts.CrateParam param)
    {
        if (this.vmCutsCreateParam is null)
        {
            param = default;
            return false;
        }

        param = this.vmCutsCreateParam;
        this.vmCutsCreateParam = null;
        return true;
    }
}
