namespace SlackAssist.Fremawork.Slack;

using System.Threading.Tasks;
using SlackAssist.Fremawork.Workflow;
using SlackNet;

internal interface IWorkflowStep
{
    string StepCallbackId { get; }

    Task OnRecv(ISlackApiClient slack, FunctionExecuted slackEvent);
}