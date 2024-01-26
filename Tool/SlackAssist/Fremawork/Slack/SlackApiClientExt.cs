namespace SlackAssist.Fremawork.Slack;

using System.Threading.Tasks;
using SlackNet;

internal static class SlackApiClientExt
{
    public static async Task<string> GetUserName(this ISlackApiClient slack, string userId)
    {
        var user = await slack.Users.Info(userId);
        if (user is null)
        {
            return string.Empty;
        }

        return user.Name;
    }

    public static async Task<string> GetChannelName(this ISlackApiClient slack, string channelId)
    {
        var channel = await slack.Conversations.Info(channelId);
        if (channel is null)
        {
            return string.Empty;
        }

        return channel.Name;
    }
}
