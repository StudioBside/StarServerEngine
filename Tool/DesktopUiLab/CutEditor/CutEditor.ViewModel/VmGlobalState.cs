namespace CutEditor.ViewModel;

using System.Reflection;
using Cs.Core;
using Cs.Core.Util;
using CutEditor.ViewModel.Detail;
using Microsoft.Extensions.Configuration;

public sealed class VmGlobalState
{
    public static VmGlobalState Instance => Singleton<VmGlobalState>.Instance;
    public string TextFilePath { get; private set; } = string.Empty;

    public void Initialize(IConfiguration config)
    {
        var templateSource = Assembly.GetExecutingAssembly().GetResourceString("CutEditor.ViewModel.TextTemplates.CutsOutput.stg");
        StringTemplateFactory.Instance.CreateFromString("CutsOutput", templateSource);

        this.TextFilePath = config["CutTextFilePath"] ?? throw new Exception("CutTextFilePath is not set in the configuration file.");
    }
}
