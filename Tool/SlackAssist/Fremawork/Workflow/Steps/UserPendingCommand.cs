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

internal class UserPendingCommand : IWorkflowStep
{
    private const string TargetUserInput = "target_user";
    private const string TargetUserOutput = "sent_target_user";
    private const string CommandTextInput = "command_text";
    private const string CommandTextOutput = "sent_command_text";

    public string StepCallbackId => "user_pending_command";
    private string DebugName => $"[UserPendingCommand]";

    public async Task OnRecv(ISlackApiClient slack, FunctionExecuted slackEvent)
    {
        var users = slackEvent.Inputs[TargetUserInput].Split(",");
        var commandText = slackEvent.Inputs[CommandTextInput];

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

        foreach (var user in users)
        {
            var userName = await slack.GetUserName(user);
            Log.Debug($"{this.DebugName} targetUser:{userName} commandText:{commandText} executeId:{slackEvent.FunctionExecutionId}");

            var message = await jobCommand.Process(slack, arguments);
            message.Channel = user;
            await slack.Chat.PostMessage(message);
        }

        await slack.WorkflowSuccess(slackEvent.FunctionExecutionId, new Dictionary<string, string>
        {
            { TargetUserOutput, string.Join(", ", users) },
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
                Channel = "@choisungki",
            };
            await slack.Chat.PostMessage(slackResponse);

            await slack.WorkflowError(slackEvent.FunctionExecutionId, errorMessage);
        }
    }
}
