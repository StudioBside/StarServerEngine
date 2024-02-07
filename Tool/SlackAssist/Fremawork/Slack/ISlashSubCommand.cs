namespace SlackAssist.Fremawork.Slack;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cs.Messaging;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal interface ISlashSubCommand
{
    string Command { get; }
    IEnumerable<string> CommandLiterals { get; }
    Task<Message> Process(ISlackApiClient slack, SlashCommand command, IReadOnlyList<string> arguments);
    Block GetIntroduceBlock();
}