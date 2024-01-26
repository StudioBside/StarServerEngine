namespace SlackAssist.Fremawork.Slack;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;

internal interface IChatReaction
{
    TargetEventRecord? IsTargetEvent(MessageEvent slackEvent);
    Task Process(ISlackApiClient slack, TargetEventRecord eventRecord);
    Block GetIntroduceBlock();
}

internal record TargetEventRecord(MessageEvent OriginalEvent, MessageEvent TargetEvent)
{
    public Match? RegexMatch { get; init; }
}
