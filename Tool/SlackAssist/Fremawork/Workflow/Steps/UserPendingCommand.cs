namespace SlackAssist.Fremawork.Workflow;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cs.Logging;
using SlackAssist.Fremawork.Slack;
using SlackAssist.SlackNetHandlers;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal class UserPendingCommand : IWorkflowStep
{
    private const string TargetUserAction = "target_user_action";
    private const string TargetUserInput = "target_user";
    private const string TargetUserOutput = "sent_target_user";
    private const string CommandTextAction = "command_text_action";
    private const string CommandTextInput = "command_text";
    private const string CommandTextOutput = "sent_command_text";

    public string StepCallbackId => "user_pending_command";
    public string ConfigCallbackId => "user_pending_command_config";
    public ISlackApiClient Slack { get; set; } = null!;
    private string DebugName => $"[UserPendingCommand]";

    async Task IWorkflowStepEditHandler.Handle(WorkflowStepEdit workflowStepEdit)
    {
        var userName = await this.Slack.GetUserName(workflowStepEdit.User.Id);
        Log.Debug($"{this.DebugName} {userName} opened the configuration dialog");

        await this.Slack.Views.Open(workflowStepEdit.TriggerId, new ConfigurationModalViewDefinition
        {
            CallbackId = this.ConfigCallbackId,
            Blocks =
            {
                new InputBlock
                {
                    Label = "결과를 전송할 사용자 선택",
                    Element = new UserMultiSelectMenu
                    {
                        ActionId = TargetUserAction,
                        InitialUsers = workflowStepEdit.WorkflowStep.Inputs.TryGetValue(TargetUserInput, out var user) ? user.Value.Split(",") : null,
                    },
                }, new InputBlock
                {
                    Label = "예약할 명령어 입력",
                    Element = new PlainTextInput
                    {
                        ActionId = CommandTextAction,
                        InitialValue = workflowStepEdit.WorkflowStep.Inputs.TryGetValue(CommandTextInput, out var input) ? input.Value : string.Empty,
                    },
                },
            },
        });
    }

    async Task<ViewSubmissionResponse> IViewSubmissionHandler.Handle(ViewSubmission viewSubmission)
    {
        Log.Debug($"{this.DebugName} {viewSubmission.User.Name} submitted the configuration dialog");

        var viewState = viewSubmission.View.State;
        await this.Slack.Workflows.UpdateStep(
            viewSubmission.WorkflowStep.WorkflowStepEditId,
            new Dictionary<string, WorkflowInput>
            {
                { TargetUserInput, new WorkflowInput { Value = string.Join(",", viewState.GetValue<UserMultiSelectValue>(TargetUserAction).SelectedUsers) } },
                { CommandTextInput, new WorkflowInput { Value = viewState.GetValue<PlainTextInputValue>(CommandTextAction).Value } },
            },
            new List<WorkflowOutput>
            {
                new() { Label = "Target User", Name = TargetUserOutput, Type = WorkflowOutputType.User },
                new() { Label = "Command Text", Name = CommandTextOutput, Type = WorkflowOutputType.Text },
            });
        return ViewSubmissionResponse.Null;
    }

    Task IViewSubmissionHandler.HandleClose(ViewClosed viewClosed)
    {
        Log.Debug($"{this.DebugName} {viewClosed.User.Name} cancelled the configuration dialog");
        return Task.CompletedTask;
    }

    public async Task OnRecv(WorkflowStepExecute slackEvent)
    {
        var users = slackEvent.WorkflowStep.Inputs[TargetUserInput].Value.Split(",");
        var commandText = slackEvent.WorkflowStep.Inputs[CommandTextInput].Value;

        var tokens = commandText.Split(" ");
        if (tokens.Length < 2)
        {
            await SendErrorMessage(commandText, $"명령어 구조 분석에 실패했습니다. #tokens:{tokens.Length}");
            return;
        }

        (string command, string literal) = (tokens[0], tokens[1]);
        //// 지금 command가 하나 뿐이라... 구분 처리 생략합니다.
        string[] arguments = tokens.Length >= 3 ? tokens[2..] : Array.Empty<string>();

        if (SlashCommandHandler.TryGetSubCommand(literal, out var subCommand) == false)
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
            var userName = await this.Slack.GetUserName(user);
            Log.Debug($"{this.DebugName} targetUser:{userName} commandText:{commandText} executeId:{slackEvent.WorkflowStep.WorkflowStepExecuteId}");

            var message = await jobCommand.Process(this.Slack, arguments);
            message.Channel = user;
            await this.Slack.Chat.PostMessage(message);
        }

        await this.Slack.Workflows.StepCompleted(slackEvent.WorkflowStep.WorkflowStepExecuteId, new Dictionary<string, string>
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
            await this.Slack.Chat.PostMessage(slackResponse);
        }
    }
}
