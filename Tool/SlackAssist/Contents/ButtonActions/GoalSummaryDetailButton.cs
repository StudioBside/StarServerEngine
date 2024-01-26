namespace SlackAssist.Contents.ButtonActions;

using System.Linq;
using System.Threading.Tasks;

using Cs.Logging;

using SlackAssist.Contents.SlashSlackAssist;
using SlackAssist.Fremawork.Slack;

using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;

internal class GoalSummaryDetailButton : IButtonAction
{
    public string ActionId => "goal_summary";

    public static SlackNet.Blocks.Button Create(string text, string version)
    {
        return IButtonAction.MakeActionButton(new GoalSummaryDetailButton(), text, version);
    }

    public async Task Process(ISlackApiClient slack, ButtonAction action, BlockActionRequest request)
    {
        var arguments = action.Value.Split(FormProcessing.Separator);
        if (arguments.Any() == false)
        {
            Log.Warn($"not enough argument. argumentCount:{arguments.Length}");
            return;
        }

        var result = new RedmineList();
        var message = await result.Process(slack, arguments);
        message.IconEmoji = ":redmine:";
        message.Username = "Redmine";
        message.Channel = request.Channel.Id;

        await slack.Chat.PostEphemeral(request.User.Id, message);
    }
}
