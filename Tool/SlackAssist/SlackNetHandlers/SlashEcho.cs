namespace SlackAssist.Handlers;

using System;
using System.Threading.Tasks;
using SlackNet;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal sealed class SlashEcho : ISlashCommandHandler
{
    public const string SlashCommand = "/echo";

    private readonly ISlackApiClient slack;
    public SlashEcho(ISlackApiClient slack) => this.slack = slack;

    public async Task<SlashCommandResponse> Handle(SlashCommand command)
    {
        Console.WriteLine($"{command.UserName} used the {SlashCommand} slash command in the {command.ChannelName} channel");

        await Task.Delay(0);
        return new SlashCommandResponse
        {
            Message = new Message { Text = command.Text },
        };
    }
}
