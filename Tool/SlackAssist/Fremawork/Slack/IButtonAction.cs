namespace SlackAssist.Fremawork.Slack;

using System.Threading.Tasks;

using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;

internal interface IButtonAction
{
    string ActionId { get; }

    Task Process(ISlackApiClient slack, ButtonAction action, BlockActionRequest request);

    static SlackNet.Blocks.Button MakeUrlButton(string text, string url)
    {
        return new SlackNet.Blocks.Button
        {
            Text = text,
            ActionId = "url",
            Url = url,
        };
    }

    static SlackNet.Blocks.Button MakeActionButton(IButtonAction action, string text, params string[] args)
    {
        return new SlackNet.Blocks.Button
        {
            Text = text,
            ActionId = action.ActionId,
            Value = string.Join(FormProcessing.Separator, args),
        };
    }
}
