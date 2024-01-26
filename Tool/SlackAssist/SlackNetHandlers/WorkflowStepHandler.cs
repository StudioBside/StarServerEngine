namespace SlackAssist.SlackNetHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cs.Dynamic;
using SlackAssist.Fremawork.Slack;
using SlackNet;
using SlackNet.Events;

internal class WorkflowStepHandler : IEventHandler<WorkflowStepExecute>
{
    private static readonly Dictionary<string /*stepCallbackId*/, IWorkflowStep> Handlers = new();

    public static void Initialize(SlackServiceBuilder slackServices, ISlackApiClient slack)
    {
        var handlerTypes = Assembly.GetExecutingAssembly()
           .GetTypes()
           .Where(Filter);

        foreach (var type in handlerTypes)
        {
            var handler = (IWorkflowStep)type.CreateInstance();
            handler.Slack = slack;

            slackServices
                .RegisterWorkflowStepEditHandler(handler.StepCallbackId, ctx => handler)
                .RegisterViewSubmissionHandler(handler.ConfigCallbackId, ctx => handler)
                ;

            Handlers.Add(handler.StepCallbackId, handler);
        }

        static bool Filter(Type type)
        {
            return type.GetInterface(nameof(IWorkflowStep)) != null;
        }
    }

    public Task Handle(WorkflowStepExecute slackEvent)
    {
        if (Handlers.TryGetValue(slackEvent.CallbackId, out var workflowStep) == false)
        {
            return Task.CompletedTask;
        }

        return workflowStep.OnRecv(slackEvent);
    }
}
