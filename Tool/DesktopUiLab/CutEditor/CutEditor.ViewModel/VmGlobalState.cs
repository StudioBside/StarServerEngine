namespace CutEditor.ViewModel;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Cs.Core;
using Cs.Core.Util;
using CutEditor.ViewModel.Detail;

public sealed class VmGlobalState
{
    public static VmGlobalState Instance => Singleton<VmGlobalState>.Instance;
    public VmCuts.CrateParam? VmCutsCreateParam { get; private set; }

    public void Initialize()
    {
        var templateSource = Assembly.GetExecutingAssembly().GetResourceString("CutEditor.ViewModel.TextTemplates.CutsOutput.stg");
        StringTemplateFactory.Instance.CreateFromString("CutsOutput", templateSource);
    }

    internal void ReserveVmCuts(VmCuts.CrateParam param)
    {
        // 마지막으로 reserve한 param은 계속 유지. history back -> forward 시에 사용
        this.VmCutsCreateParam = param;
    }
}
