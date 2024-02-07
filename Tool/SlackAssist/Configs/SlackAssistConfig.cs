namespace SlackAssist.Configs
{
    using System.Collections.Generic;
    using Cs.Logging;
    using Microsoft.Extensions.Configuration;

    internal sealed class SlackAssistConfig
    {
        public static SlackAssistConfig Instance { get; private set; } = null!;

        internal SlackConfig Slack { get; private set; } = null!;
        internal RedmineConfig Redmine { get; private set; } = null!;
        internal GameRankConfig GameRank { get; private set; } = null!;
        internal string LogPath { get; private set; } = string.Empty;

        public static SlackAssistConfig? Load(IConfiguration config)
        {
            var target = config.GetValue<string>("target");
            if (string.IsNullOrEmpty(target))
            {
                target = "StarSavior";
            }

            Log.Info($"Configuration target: {target}");
            var targetConfig = config.GetRequiredSection(target);

            var result = targetConfig.Get<SlackAssistConfig>(e =>
            {
                e.BindNonPublicProperties = true;
                e.ErrorOnUnknownConfiguration = true;
            });

            if (result is null)
            {
                return null;
            }

            Instance = result;
            if (string.IsNullOrEmpty(Instance.LogPath))
            {
                Instance.LogPath = "./";
            }

            return Instance;
        }

        internal sealed record SlackConfig
        {
            public required string ApiBotToken { get; init; }
            public required string AppLevelToken { get; init; }
            public required string SlashCommandPrefix { get; init; }
        }

        internal sealed record RedmineConfig
        {
            public required string ServerAddress { get; init; }
            public required string ApiKey { get; init; }

            public List<string> InvisiableUsers { get; init; } = new();
        }

        internal sealed record GameRankConfig
        {
            public required string StoragePath { get; init; }
        }
    }
}