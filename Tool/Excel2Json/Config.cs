namespace Excel2Json;

using Cs.Core.Util;
using Cs.Logging;

internal sealed class Config
{
    public static Config Instance { get; private set; } = null!;

    public PathConfig Path { get; set; } = null!;

    public string TextOutputExtension { get; set; } = string.Empty;
    public string BinOutputExtension { get; set; } = string.Empty;
    public string CSharpOutputExtension { get; set; } = string.Empty;
    public bool UseFileTypePrefix { get; set; }
    public bool UseFileDirectionPrefix { get; set; }
    public string DeleteTargetExtension { get; set; } = string.Empty;

    public static bool Initiaize()
    {
        Instance = JsonUtil.Load<Config>("excel2json.config.json");

        return Instance.Validate();
    }

    private bool Validate()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        Log.Info(Log.BuildHead("Config"));
        Log.Info($"경로 정보 수집. 현재 실행 경로:{currentDirectory}");

        bool pathValidation
            = ValidatePath(this.Path.BindingRule, nameof(this.Path.BindingRule))
            && ValidatePath(this.Path.TextTemplate, nameof(this.Path.TextTemplate))
            && ValidatePath(this.Path.ExcelInput, nameof(this.Path.ExcelInput))
            && ValidatePath(this.Path.ServerTextOutput, nameof(this.Path.ServerTextOutput))
            && ValidatePath(this.Path.ServerBinOutput, nameof(this.Path.ServerBinOutput))
            //&& ValidatePath(this.Path.ServerEnumOutput, nameof(this.Path.ServerEnumOutput))
            && ValidatePath(this.Path.ClientTextOutput, nameof(this.Path.ClientTextOutput))
            && ValidatePath(this.Path.ClientBinOutput, nameof(this.Path.ClientBinOutput))
            && ValidatePath(this.Path.ClientEnumOutput, nameof(this.Path.ClientEnumOutput))
            && ValidatePath(this.Path.ToolTextOutput, nameof(this.Path.ToolTextOutput))
            && ValidatePath(this.Path.ToolBinOutput, nameof(this.Path.ToolBinOutput))
            && ValidatePath(this.Path.ToolEnumOutput, nameof(this.Path.ToolEnumOutput))
            ;

        return pathValidation;

        static bool ValidatePath(string relPath, string pathName)
        {
            if (Directory.Exists(relPath) == false)
            {
                Log.Error($"설정 파일의 경로가 올바르지 않습니다. {pathName}:{relPath}");
                return false;
            }

            Log.Debug($"{pathName}:{relPath} ... OK");
            return true;
        }
    }

    public sealed class PathConfig
    {
        public string BindingRule { get; set; } = string.Empty;
        public string TextTemplate { get; set; } = string.Empty;
        public string ExcelInput { get; set; } = string.Empty;
        public string ServerTextOutput { get; set; } = string.Empty;
        public string ServerBinOutput { get; set; } = string.Empty;
        //public string ServerEnumOutput { get; set; } = string.Empty;
        public string ClientTextOutput { get; set; } = string.Empty;
        public string ClientBinOutput { get; set; } = string.Empty;
        public string ClientEnumOutput { get; set; } = string.Empty;
        public string ToolTextOutput { get; set; } = string.Empty;
        public string ToolBinOutput { get; set; } = string.Empty;
        public string ToolEnumOutput { get; set; } = string.Empty;

        public string HotswapEntityOutput { get; set; } = string.Empty;
    }
}
