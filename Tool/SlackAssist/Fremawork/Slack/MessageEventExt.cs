namespace SlackAssist.Fremawork.Slack;

using System.Threading.Tasks;
using SlackNet;
using SlackNet.Events;

internal static class MessageEventExt
{
    //@ return ex: choisungki
    public static async Task<string> GetUserNameAsync(this MessageEvent slackEvent, ISlackApiClient slack)
    {
        if (string.IsNullOrEmpty(slackEvent.User))
        {
            return string.Empty;
        }

        var user = await slack.Users.Info(slackEvent.User);
        if (user is null)
        {
            return string.Empty;
        }

        return user.Name;
    }

    //@ return ex: 최성기
    public static async Task<string> GetUserRealNameAsync(this MessageEvent slackEvent, ISlackApiClient slack)
    {
        if (string.IsNullOrEmpty(slackEvent.User))
        {
            return string.Empty;
        }

        var user = await slack.Users.Info(slackEvent.User);
        if (user is null)
        {
            return string.Empty;
        }

        return user.RealName;
    }

    public static async Task<string> GetUserNameAsync(this AppHomeOpened slackEvent, ISlackApiClient slack)
    {
        if (string.IsNullOrEmpty(slackEvent.User))
        {
            return string.Empty;
        }

        var user = await slack.Users.Info(slackEvent.User);
        if (user is null)
        {
            return string.Empty;
        }

        return user.Name;
    }

    public static async Task<string> GetChannelNameAsync(this MessageEvent slackEvent, ISlackApiClient slack)
    {
        if (string.IsNullOrEmpty(slackEvent.Channel))
        {
            return string.Empty;
        }

        var conversation = await slack.Conversations.Info(slackEvent.Channel);
        if (conversation is null)
        {
            return string.Empty;
        }

        return conversation.Name;
    }
}
