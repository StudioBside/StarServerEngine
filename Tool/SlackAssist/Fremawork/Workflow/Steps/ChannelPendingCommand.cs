namespace SlackAssist.Fremawork.Workflow.Steps;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cs.Logging;
using SlackAssist.Fremawork.Slack;
using SlackAssist.SlackNetHandlers;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

internal class ChannelPendingCommand : IWorkflowStep
{
    private const string TargetChannelInput = "target_channel";
    private const string TargetChannelOutput = "sent_target_channel";
    private const string CommandTextInput = "command_text";
    private const string CommandTextOutput = "sent_command_text";

    public string StepCallbackId => "workflow_command";
    private string DebugName => $"[WorkflowCommand]";

    public async Task OnRecv(ISlackApiClient slack, FunctionExecuted slackEvent)
    {
        var channel = slackEvent.Inputs[TargetChannelInput];
        var commandText = slackEvent.Inputs[CommandTextInput];

        var channelName = await slack.GetChannelName(channel);
        Log.Debug($"{this.DebugName} targetChannel:{channelName} commandText:{commandText} executeId:{slackEvent.FunctionExecutionId}");

        var tokens = commandText.Split(" ");
        if (tokens.Length < 2)
        {
            await SendErrorMessage(commandText, $"명령어 구조 분석에 실패했습니다. #tokens:{tokens.Length}");
            return;
        }

        (string command, string literal) = (tokens[0], tokens[1]);
        //// 지금 command가 하나 뿐이라... 구분 처리 생략합니다.
        string[] arguments = tokens.Length >= 3 ? tokens[2..] : Array.Empty<string>();

        if (SlashCommandHandler.TryGetCommand(command, literal, out var subCommand) == false)
        {
            await SendErrorMessage(commandText, $"지원하지 않는 명령어 `{literal}` 입니다.");
            return;
        }

        if (subCommand is not IWorkflowCommand jobCommand)
        {
            await SendErrorMessage(commandText, "이 명령은 스케줄러에 등록할 수 없는 유형입니다.");
            return;
        }

        var message = await jobCommand.Process(slack, arguments);
        message.Channel = channel;
        await slack.Chat.PostMessage(message);

        await slack.WorkflowSuccess(slackEvent.FunctionExecutionId, new Dictionary<string, string>
        {
            { TargetChannelOutput, channel },
            { CommandTextOutput, commandText },
        });

        async Task SendErrorMessage(string commandText, string errorMessage)
        {
            Log.Error($"{this.DebugName} commandText:{commandText} errorMessage:{errorMessage}");

            var slackResponse = new Message
            {
                IconEmoji = ":redmine:",
                Username = "Redmine",
                Text = $":x: 워크플로우 명령 실행 실패. 명령:{commandText} 오류:{errorMessage}",
                Channel = channel,
            };
            await slack.Chat.PostMessage(slackResponse);

            await slack.WorkflowError(slackEvent.FunctionExecutionId, errorMessage);
        }
    }
}
