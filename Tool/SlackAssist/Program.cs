namespace SlackAssist;
using System.IO;
using System.Threading.Tasks;
using Cs.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // required argument
        if (args.Length < 1)
        {
            throw new DirectoryNotFoundException("Required working directory not found");
        }

        var workingDirectory = Path.GetFullPath(args[0]);
        Log.Info($"WorkingDirectory: {workingDirectory}");
        Directory.SetCurrentDirectory(workingDirectory);

        // application
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddWindowsService();
                services.AddHostedService<SlackAssistServer>();
            })
            .Build();

        await host.RunAsync();
    }
}