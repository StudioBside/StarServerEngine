namespace SlackAssist.SlackNetHandlers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Cs.Logging;
using SlackAssist.Fremawork.Slack;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;

internal sealed class AppHome : IEventHandler<AppHomeOpened>
{
    private readonly ISlackApiClient slack;

    public AppHome(ISlackApiClient slack)
    {
        this.slack = slack;
    }

    public async Task Handle(AppHomeOpened slackEvent)
    {
        if (slackEvent.Tab == AppHomeTab.Home)
        {
            var userName = await this.slack.GetUserName(slackEvent.User);
            Log.Debug($"{userName} opened the app's home view");

            var blocks = new List<Block>();
            blocks.Add(new SectionBlock { Text = new Markdown("스튜디오 비사이드에서 사용하는 개발용 봇입니다") });
            blocks.Add(new DividerBlock());

            // -------------------------------------------------------------------------------------------------------
            blocks.Add(new SectionBlock { Text = new Markdown("*이벤트 핸들러* : 채팅 메시지에 포함된 단어에 자동 반응하는 동작") });
            foreach (var handler in OnMessageEvent.MessageHandlers)
            {
                blocks.Add(handler.GetIntroduceBlock());
            }

            blocks.Add(new DividerBlock());

            // -------------------------------------------------------------------------------------------------------
            blocks.AddRange(SlashCommandHandler.GetIntroduceBlocks());

            // -------------------------------------------------------------------------------------------------------
            blocks.Add(new SectionBlock { Text = new Markdown("*워크플로우 스텝* : 워크플로우에서 `Step`으로 사용 가능한 기능") });

            await this.slack.Views.Publish(
                slackEvent.User,
                new HomeViewDefinition { Blocks = blocks },
                slackEvent.View?.Hash);
        }
    }
}
