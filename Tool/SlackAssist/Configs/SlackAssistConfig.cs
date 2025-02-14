namespace SlackAssist.Configs
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    internal sealed class SlackAssistConfig
    {
        private const string Key = nameof(SlackAssistConfig);
        public static SlackAssistConfig Instance { get; private set; } = null!;

        internal SlackConfig Slack { get; private set; } = null!;
        internal RedmineConfig Redmine { get; private set; } = null!;
        internal GameRankConfig GameRank { get; private set; } = null!;
        internal string LogPath { get; private set; } = string.Empty;

        public static SlackAssistConfig? Load(IConfiguration config)
        {
            var section = config.GetRequiredSection(Key);
            var result = section.Get<SlackAssistConfig>(e =>
            {
                e.BindNonPublicProperties = true;
                e.ErrorOnUnknownConfiguration = true;
            });

            if (result is null)
            {
                return null;
            }

            Instance = result;
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