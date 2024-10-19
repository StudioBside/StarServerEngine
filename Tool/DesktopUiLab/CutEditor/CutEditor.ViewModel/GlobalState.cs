namespace CutEditor.ViewModel;

using System;
using System.Diagnostics.CodeAnalysis;
using Cs.Core;

internal sealed class GlobalState
{
    private VmCuts.CrateParam? vmCutsCreateParam;

    public static GlobalState Instance => Singleton<GlobalState>.Instance;

    public VmCuts.CrateParam? VmCutCreateParam { get; set; }

    public void ReserveVmCuts(VmCuts.CrateParam param)
    {
        this.vmCutsCreateParam = param;
    }

    public bool PopVmCuts([MaybeNullWhen(false)] out VmCuts.CrateParam param)
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
