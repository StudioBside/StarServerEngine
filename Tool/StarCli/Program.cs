namespace StarCli;

using System.CommandLine;
using Cs.Cli;
using Microsoft.Extensions.DependencyInjection;
using StarCli.Commands;

public sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        // DI 컨테이너 설정
        var serviceProvider = new ServiceCollection()
            .AddCsDotCliServices()
            .AddTransient<ConfigCommand>()
            .AddTransient<KafkaCommand>()
            .BuildServiceProvider();

        var rootCommand = new RootCommand("[프로젝트 스타] 게임서버 및 인프라 제어 cli 인터페이스")
        {
            serviceProvider.GetRequiredService<ConfigCommand>(),
            serviceProvider.GetRequiredService<KafkaCommand>(),
        };

        return await rootCommand.InvokeAsync(args);
    }
}
