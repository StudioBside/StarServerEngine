namespace SlackAssist.Fremawork.Slack.MessageHandlerBase;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using SlackAssist.Fremawork.Slack;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;

internal abstract class RegexReaction : IChatReaction
{
    public abstract string Pattern { get; }

    public TargetEventRecord? IsTargetEvent(MessageEvent slackEvent)
    {
        Match? match = null;
        if (slackEvent is MessageChanged changedEvent)
        {
            if (TryPickPrevText(slackEvent, out var prevText) == false)
            {
                return null;
            }

            string newText = changedEvent.Message.Text;
            var isSuitable = string.Equals(prevText, newText) == false // 메시지 내용에 변경이 있고
                && this.CheckSuitable(prevText, out _) == false // 이전에는 조건에 충족하지 않았다가
                && this.CheckSuitable(newText, out match); // 새 메세지만 조건을 충족하는 경우.

            if (isSuitable == false || match is null)
            {
                return null;
            }

            return new TargetEventRecord(slackEvent, changedEvent.Message) { RegexMatch = match };
        }

        if (this.CheckSuitable(slackEvent.Text, out match) == false)
        {
            return null;
        }

        return new TargetEventRecord(slackEvent, slackEvent) { RegexMatch = match };
    }

    public abstract Task Process(ISlackApiClient slack, TargetEventRecord eventRecord);
    public abstract Block GetIntroduceBlock();

    //// ------------------------------------------------------------------------------------------

    private static bool TryPickPrevText(MessageEvent slackEvent, [MaybeNullWhen(false)] out string prevText)
    {
        if (slackEvent.ExtraProperties.TryGetValue("previous_message", out var value) == false)
        {
            prevText = default;
            return false;
        }

        if (value is not JObject jObject)
        {
            prevText = default;
            return false;
        }

        prevText = jObject.GetString("text");
        return prevText != null;
    }

    private bool CheckSuitable(string message, [MaybeNullWhen(false)] out Match match)
    {
        if (string.IsNullOrEmpty(message))
        {
            match = null;
            return false;
        }

        match = Regex.Match(message, this.Pattern);
        if (match.Success == false)
        {
            return false;
        }

        return true;
    }
}
