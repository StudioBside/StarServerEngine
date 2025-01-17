namespace Cs.Cli;

using System.CommandLine;
using Cs.Logging;

public abstract class CommandBase : Command
{
    private readonly IHomePathConfig config;
    protected CommandBase(IHomePathConfig config, string name, string description) : base(name, description)
    {
        this.config = config;
    }

    protected IHomePathConfig Config => this.config;

    protected string? GetBranchConfig(string key)
    {
        var branchName = this.config.GetValue(HomePathConfigType.All, "branch");
        if (branchName is null)
        {
            Log.Error($"브랜치 설정이 없습니다. 'star config set branch <branch>'로 설정하세요.");
            return null;
        }

        var sectionName = $"branch \"{branchName}\"";
        return this.config.GetValue(HomePathConfigType.All, $"{sectionName}.{key}");
    }
}
