namespace SlackAssist.Fremawork.Slack.ChatReactionBase;

using System.Threading.Tasks;
using SlackAssist.Fremawork.Slack;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;

internal abstract class StringContainsReaction : IChatReaction
{
    public abstract string Trigger { get; }

    public TargetEventRecord? IsTargetEvent(MessageEvent slackEvent)
    {
        if (slackEvent is MessageChanged changedEvent)
        {
            string prevText = string.Empty;
            string newText = changedEvent.Message.Text;

            var isSuitable = string.Equals(prevText, newText) == false // 메시지 내용에 변경이 있고
                && this.CheckSuitable(prevText) == false // 이전에는 조건을 충족하지 않았다가
                && this.CheckSuitable(newText); // 새 메세지만 조건을 충족하는 경우.

            if (isSuitable == false)
            {
                return null;
            }

            return new TargetEventRecord(slackEvent, changedEvent.Message);
        }

        if (this.CheckSuitable(slackEvent.Text) == false)
        {
            return null;
        }

        return new TargetEventRecord(slackEvent, slackEvent);
    }

    public abstract Task Process(ISlackApiClient slack, TargetEventRecord eventRecord);
    public abstract Block GetIntroduceBlock();

    //// ------------------------------------------------------------------------------------------

    private bool CheckSuitable(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return false;
        }

        if (message.Contains(this.Trigger, System.StringComparison.OrdinalIgnoreCase) == false)
        {
            return false;
        }

        return true;
    }
}
