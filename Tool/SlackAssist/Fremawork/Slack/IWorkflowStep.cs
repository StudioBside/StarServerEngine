namespace SlackAssist.Fremawork.Slack;

using System.Threading.Tasks;
using SlackNet;
using SlackNet.Events;
using SlackNet.Interaction;

internal interface IWorkflowStep : IWorkflowStepEditHandler, IViewSubmissionHandler
{
    string StepCallbackId { get; }
    string ConfigCallbackId { get; }
    ISlackApiClient Slack { set; }

    Task OnRecv(WorkflowStepExecute slackEvent);
}
