namespace SlackAssist.Contents.SlashCommands.Assist
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SlackAssist.Contents.Detail;
    using SlackAssist.Fremawork.Slack;
    using SlackAssist.Fremawork.Workflow;
    using SlackAssist.SlackNetHandlers;
    using SlackNet;
    using SlackNet.Blocks;
    using SlackNet.Interaction;
    using SlackNet.WebApi;

    internal sealed class GameRank : ISlashSubCommand, IWorkflowCommand
    {
        public string Command => SlashCommandHandler.BuildCommand("assist");
        public IEnumerable<string> CommandLiterals { get; } = new[] { "rank", "순위" };

        public Block GetIntroduceBlock()
        {
            var builder = new MarkdownBuilder();
            builder.WriteLine($"명령 : {string.Join(", ", this.CommandLiterals.Select(e => $"`{e}`"))} :googleplay:");
            builder.WriteLine($"효과 : 구글플레이 게임 매출 순위 정보를 표시합니다");
            builder.WriteLine($"문법 : <서브명령>");
            builder.WriteLine($"예시 : *{this.Command} rank*");

            return builder.FlushToSectionBlock();
        }

        public Task<Message> Process(ISlackApiClient slack, IReadOnlyList<string> arguments)
        {
            if (GameRankLoader.TryLoad(out var message) == false)
            {
                message = new Message { Text = "랭킹 데이터 조회에 실패했습니다." };
            }

            return Task.FromResult(message);
        }

        public Task<Message> Process(ISlackApiClient slack, SlashCommand command, IReadOnlyList<string> arguments)
        {
            return this.Process(slack, arguments);
        }
    }
}