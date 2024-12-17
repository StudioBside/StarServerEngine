namespace CutEditor.ViewModel;

using System.IO;
using System.Reflection;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.ViewModel.Detail;
using Microsoft.Extensions.Configuration;

public sealed class VmGlobalState
{
    public static VmGlobalState Instance => Singleton<VmGlobalState>.Instance;
    public string TextFilePath { get; private set; } = string.Empty;
    public string BinFilePath { get; private set; } = string.Empty;
    public string PackerExeFilePath { get; private set; } = string.Empty;
    public string CutSceneDataFilePath { get; private set; } = string.Empty;

    public void Initialize(IConfiguration config)
    {
        var templateSource = Assembly.GetExecutingAssembly().GetResourceString("CutEditor.ViewModel.TextTemplates.CutsOutput.stg");
        StringTemplateFactory.Instance.CreateFromString("CutsOutput", templateSource);

        this.TextFilePath = config["CutTextFilePath"] ?? throw new Exception("CutTextFilePath is not set in the configuration file.");
        this.BinFilePath = config["CutBinFilePath"] ?? throw new Exception("CutBinFilePath is not set in the configuration file.");
        this.PackerExeFilePath = config["TextFilePacker"] ?? throw new Exception("TextFilePacker is not set in the configuration file.");
        this.CutSceneDataFilePath = config["CutSceneDataFile"] ?? throw new Exception("CutSceneDataFile is not set in the configuration file.");

        if (!File.Exists(this.CutSceneDataFilePath))
        {
            Log.ErrorAndExit($"cutscene file not found: {this.CutSceneDataFilePath}");
        }
    }
}
