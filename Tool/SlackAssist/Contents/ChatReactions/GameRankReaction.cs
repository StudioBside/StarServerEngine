namespace SlackAssist.Contents.ChatReactions;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs.Logging;
using GameRank.Core;
using SlackAssist.Configs;
using SlackAssist.Contents.Detail;
using SlackAssist.Fremawork.Slack;
using SlackAssist.Fremawork.Slack.MessageHandlerBase;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;

internal class GameRankReaction : StringContainsReaction
{
    public override string Trigger => "[매출 순위]";
    private string DebugName => $"[MobileGameRank]";

    public override Block GetIntroduceBlock()
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"트리거 : `{this.Trigger}`");
            writer.WriteLine($"효과 : 현재 모바일 게임 순위 정보를 보여줍니다. (한국 구글플레이 매출 한정)");
            writer.WriteLine($"예시 : .. 이는 현재 *[매출 순위]* 에서 세나 키우기의 흥행 실적으로도 나타납니다. 반면 ...");
        }

        return new SectionBlock { Text = new Markdown(sb.ToString()) };
    }

    public override async Task Process(ISlackApiClient slack, TargetEventRecord eventRecord)
    {
        var targetEvent = eventRecord.TargetEvent;
        string userName = await targetEvent.GetUserNameAsync(slack);
        string channelName = await eventRecord.OriginalEvent.GetChannelNameAsync(slack);

        Log.Debug($"{this.DebugName} message:{targetEvent.Text} userName:{userName} channelName:{channelName}");

        if (GameRankLoader.TryLoad(out var message) == false)
        {
            await slack.Chat.PostMessage(new Message
            {
                Text = "랭킹 데이터 조회에 실패했습니다.",
                Channel = eventRecord.OriginalEvent.Channel,
                ThreadTs = targetEvent.Ts,
            });

            return;
        }

        message.Channel = eventRecord.OriginalEvent.Channel;
        message.ThreadTs = targetEvent.Ts;

        await slack.Chat.PostMessage(message);
    }
}
