namespace Cs.Core.DotNetHost;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public abstract class SingleServiceBase(IConfiguration config, IHostApplicationLifetime appLifetime)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.Execute();

        // IHost의 로그 출력을 떨어뜨리기 위해 개행을 몇 자 찍어줍니다.
        Log.Info(string.Empty);
        Log.Info(string.Empty);

        appLifetime.StopApplication(); // 프로세스 종료 이벤트 트리거
    }
     
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    //// -------------------------------------------------------------------

    protected static void WriteTitle(string title)
    {
        Log.Info(string.Empty);
        Log.Info(Log.BuildHead($"[{title}]"));
        Log.Info(string.Empty);
    }

    protected abstract Task Execute();

    protected IConfigurationSection? GetTargettingSection()
    {
        var target = config["target"]; // config from command-line
        if (string.IsNullOrEmpty(target))
        {
            Log.Error("target is empty");
            return null;
        }

        Log.Info($"target: {target}");
        var configSection = config.GetSection(target);
        if (configSection.GetChildren().Any() == false)
        {
            Log.Error($"configSection.GetChildren() failed. target:{target}");
            return null;
        }

        return configSection;
    }
}
