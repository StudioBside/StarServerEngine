namespace SlackAssist
{
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Cs.Logging;
    using Cs.Logging.Providers;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using SlackAssist.Configs;
    using SlackAssist.Fremawork.Redmines;
    using SlackAssist.Fremawork.Slack;
    using SlackAssist.SlackNetHandlers;
    using SlackNet;
    using SlackNet.Blocks;
    using SlackNet.Handlers;

    internal sealed class SlackAssistServer(IConfiguration configuration, IHostApplicationLifetime appLifetime)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = SlackAssistConfig.Load(configuration);
            if (config is null)
            {
                appLifetime.StopApplication();
                return;
            }

            var logPath = Path.Join(config.LogPath, "Log.txt");
            Log.Initialize(new SimpleFileLogProvider(logPath, writeToConsole: true), LogLevelConfig.All);

            var slackServices = new SlackServiceBuilder()
                .UseApiToken(config.Slack.ApiBotToken)
                .UseAppLevelToken(config.Slack.AppLevelToken)
                .UseTypeResolver(ctx => Default.SlackTypeResolver(typeof(Default).GetTypeInfo().Assembly, Assembly.GetExecutingAssembly()))
                .UseTypeResolver(_ => Default.SlackTypeResolver(typeof(Default).GetTypeInfo().Assembly, Assembly.GetExecutingAssembly()))
                .UseLogger(ctx => new DebugDumpLogger())
                .RegisterEventHandler(cts => new AppHome(cts.ServiceProvider.GetApiClient()))
                .RegisterEventHandler(ctx => new OnMessageEvent(ctx.ServiceProvider.GetApiClient()))
                .RegisterBlockActionHandler(ctx => new TypedBlockActionHandler<ButtonAction>(new OnBlockButtonEvent(ctx.ServiceProvider.GetApiClient())))
                .RegisterEventHandler(ctx => new WorkflowStepHandler(ctx.ServiceProvider.GetApiClient()))
                ;

            SlashCommandHandler.Initialize(slackServices, config.Slack.SlashCommandPrefix);

            // note: slack의 connection은 끊어졌다가 재연결 하거나 할 수 있기 떄문에
            var apiClient = slackServices.GetApiClient();
            OnMessageEvent.Initialize();
            OnBlockButtonEvent.Initialize();
            await Redmine.Instance.Initialze(config.Redmine);

            WorkflowStepHandler.Initialize();

            Log.Debug("connecting to slack...");

            var client = slackServices.GetSocketModeClient();

            var connectionOptions = new SlackNet.SocketMode.SocketModeConnectionOptions
            {
                // note: default=2, min=1, max=10
                NumberOfConnections = 5,
            };
            await client.Connect(connectionOptions, CancellationToken.None);

            Log.Debug("connecting to slack finished.");
        }
    }
}