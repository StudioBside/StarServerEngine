namespace SlackAssist
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cs.Logging;
    using Cs.ServerEngine.Util;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using SlackAssist.Configs;
    using SlackAssist.Fremawork.Redmines;
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

            Log.Initialize(new LogProvider(config.LogPath, "Log.txt", writeTimeAndLevel: true), LogLevelConfig.All);

            var slackServices = new SlackServiceBuilder()
                .UseApiToken(config.Slack.ApiBotToken)
                .UseAppLevelToken(config.Slack.AppLevelToken)
                .RegisterEventHandler(cts => new AppHome(cts.ServiceProvider.GetApiClient()))
                .RegisterEventHandler(ctx => new OnMessageEvent(ctx.ServiceProvider.GetApiClient()))
                .RegisterBlockActionHandler(ctx => new TypedBlockActionHandler<ButtonAction>(new OnBlockButtonEvent(ctx.ServiceProvider.GetApiClient())))
                .RegisterEventHandler(ctx => new WorkflowStepHandler())
                ;

            var apiClient = slackServices.GetApiClient();
            OnMessageEvent.Initialize();
            OnBlockButtonEvent.Initialize();
            await Redmine.Instance.Initialze(config.Redmine);

            SlashCommandHandler.Initialize(slackServices, apiClient, config.Slack.SlashCommandPrefix);
            WorkflowStepHandler.Initialize(slackServices, apiClient);

            Log.Debug("connecting to slack...");

            var client = slackServices.GetSocketModeClient();
            await client.Connect();

            Log.Debug("connecting to slack finished.");
        }
    }
}