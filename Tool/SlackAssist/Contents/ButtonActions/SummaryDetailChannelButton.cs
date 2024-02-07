namespace SlackAssist.Contents.ButtonActions;

using System.Threading.Tasks;

using Cs.Logging;
using SlackAssist.Contents.SlashCommands.Redmine;
using SlackAssist.Fremawork.Slack;

using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;

internal class SummaryDetailChannelButton : IButtonAction
{
    public string ActionId => "goal_summary_channel";

    public static SlackNet.Blocks.Button Create(string text, string version)
    {
        return IButtonAction.MakeActionButton(new SummaryDetailChannelButton(), text, version);
    }

    public async Task Process(ISlackApiClient slack, ButtonAction action, BlockActionRequest request)
    {
        var arguments = action.Value.Split(FormProcessing.Separator);
        if (arguments.Length <= 0)
        {
            Log.Warn($"not enough argument. argumentCount:{arguments.Length}");
            return;
        }

        var result = new VersionDetail();
        var message = await result.Process(slack, arguments);
        message.IconEmoji = ":redmine:";
        message.Username = "Redmine";
        message.Channel = request.Channel.Id;

        await slack.Chat.PostMessage(message);
    }
}
