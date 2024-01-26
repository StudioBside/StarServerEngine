namespace SlackAssist.Fremawork.Slack;

using System.Collections.Generic;
using System.Threading.Tasks;
using SlackAssist.SlackNetHandlers;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal interface ISlashSubCommand
{
    SlashCommandCategory Category { get; }
    IEnumerable<string> CommandLiterals { get; }

    Task<Message> Process(ISlackApiClient slack, SlashCommand command, IReadOnlyList<string> arguments);
    Block GetIntroduceBlock();
}