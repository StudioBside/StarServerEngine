namespace SlackAssist.SlackNetHandlers
{
    using SlackAssist.Configs;

    internal enum SlashCommandCategory
    {
        Assist,
        Redmine,
    }

    internal static class SlashCommandCategoryExt
    {
        public static string GetMainCommand(this SlashCommandCategory category)
        {
            var prefix = SlackAssistConfig.Instance.Slack.SlashCommandPrefix;
            if (SlashCommandHandler.UseDefaultCommand)
            {
                return SlashCommandHandler.DefaultCommand;
            }

            return $"/{prefix}{category.ToString().ToLowerInvariant()}";
        }
    }
}
