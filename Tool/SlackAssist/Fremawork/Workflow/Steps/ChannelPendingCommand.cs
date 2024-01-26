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

internal class ChannelPendingCommand : IWorkflowStep
{
    private const string TargetChannelAction = "target_channel_action";
    private const string TargetChannelInput = "target_channel";
    private const string TargetChannelOutput = "sent_target_channel";
    private const string CommandTextAction = "command_text_action";
    private const string CommandTextInput = "command_text";
    private const string CommandTextOutput = "sent_command_text";

    public string StepCallbackId => "workflow_command";
    public string ConfigCallbackId => "workflow_command_config";
    public ISlackApiClient Slack { get; set; } = null!;
    private string DebugName => $"[WorkflowCommand]";

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
                    Label = "결과를 출력할 채널 선택",
                    Element = new ChannelSelectMenu
                    {
                        ActionId = TargetChannelAction,
                        InitialChannel = workflowStepEdit.WorkflowStep.Inputs.TryGetValue(TargetChannelInput, out var channel) ? channel.Value : null,
                    },
                },
                new InputBlock
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
                { TargetChannelInput, new WorkflowInput { Value = viewState.GetValue<ChannelSelectValue>(TargetChannelAction).SelectedChannel } },
                { CommandTextInput, new WorkflowInput { Value = viewState.GetValue<PlainTextInputValue>(CommandTextAction).Value } },
            },
            new List<WorkflowOutput>
            {
                new() { Label = "Target Channel", Name = TargetChannelOutput, Type = WorkflowOutputType.Channel },
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
        var channel = slackEvent.WorkflowStep.Inputs[TargetChannelInput].Value;
        var commandText = slackEvent.WorkflowStep.Inputs[CommandTextInput].Value;

        var channelName = await this.Slack.GetChannelName(channel);
        Log.Debug($"{this.DebugName} targetChannel:{channelName} commandText:{commandText} executeId:{slackEvent.WorkflowStep.WorkflowStepExecuteId}");

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

        var message = await jobCommand.Process(this.Slack, arguments);
        message.Channel = channel;
        await this.Slack.Chat.PostMessage(message);

        await this.Slack.Workflows.StepCompleted(slackEvent.WorkflowStep.WorkflowStepExecuteId, new Dictionary<string, string>
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
            await this.Slack.Chat.PostMessage(slackResponse);
        }
    }
}
