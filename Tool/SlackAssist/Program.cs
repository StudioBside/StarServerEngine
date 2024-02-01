namespace SlackAssist;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                // 실행 파일의 경로 가져오기
                var exePath = AppDomain.CurrentDomain.BaseDirectory;

                config
                    .SetBasePath(exePath) // 설정 파일의 기본 경로 설정
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddWindowsService();
                services.AddHostedService<SlackAssistServer>();
            })
            .Build();

        await host.RunAsync();
    }
}
