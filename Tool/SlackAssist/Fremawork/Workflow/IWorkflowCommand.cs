namespace SlackAssist.Fremawork.Workflow;

using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet;
using SlackNet.WebApi;

internal interface IWorkflowCommand
{
    Task<Message> Process(ISlackApiClient slack, IReadOnlyList<string> arguments);
}
