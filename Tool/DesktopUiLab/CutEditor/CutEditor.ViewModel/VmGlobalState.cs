namespace CutEditor.ViewModel;

using System.IO;
using System.Reflection;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.ViewModel.Detail;
using Microsoft.Extensions.Configuration;

public sealed class VmGlobalState
{
    public const string ExportRoot = "./Export";
    public const string ShortenExportFileName = "SHORTEN_CUT_ALL.xlsx";

    private string textfilepath = string.Empty;
    private string binfilepath = string.Empty;

    private string shortenTextFilePath = string.Empty;
    private string shortenBinFilePath = string.Empty;

    public static VmGlobalState Instance => Singleton<VmGlobalState>.Instance;
    public string PackerExeFilePath { get; private set; } = string.Empty;
    public string CutSceneDataFilePath { get; private set; } = string.Empty;

    public void Initialize(IConfiguration config)
    {
        var templateSource = Assembly.GetExecutingAssembly().GetResourceString("CutEditor.ViewModel.TextTemplates.CutsOutput.stg");
        StringTemplateFactory.Instance.CreateFromString("CutsOutput", templateSource);

        this.textfilepath = config["CutTextFilePath"] ?? throw new Exception("CutTextFilePath is not set in the configuration file.");
        this.binfilepath = config["CutBinFilePath"] ?? throw new Exception("CutBinFilePath is not set in the configuration file.");
        this.PackerExeFilePath = config["TextFilePacker"] ?? throw new Exception("TextFilePacker is not set in the configuration file.");
        this.CutSceneDataFilePath = config["CutSceneDataFile"] ?? throw new Exception("CutSceneDataFile is not set in the configuration file.");

        if (!File.Exists(this.CutSceneDataFilePath))
        {
            Log.ErrorAndExit($"cutscene file not found: {this.CutSceneDataFilePath}");
        }

        if (File.Exists(this.PackerExeFilePath) == false)
        {
            Log.ErrorAndExit($"TextFilePacker does not exist. config:{this.PackerExeFilePath}");
        }

        this.shortenTextFilePath = this.textfilepath.Replace("/CUT/", "/SHORTEN_CUT/");
        this.shortenBinFilePath = this.binfilepath.Replace("/CUT/", "/SHORTEN_CUT/");
    }

    public string GetTextFilePath(bool isShorten)
    {
        return isShorten ? this.shortenTextFilePath : this.textfilepath;
    }

    public string GetBinFilePath(bool isShorten)
    {
        return isShorten ? this.shortenBinFilePath : this.binfilepath;
    }
}
