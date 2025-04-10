namespace SlackAssist.SlackNetHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cs.Core.Util;
using Cs.Dynamic;
using SlackAssist.Fremawork.Slack;
using SlackAssist.Fremawork.Workflow;
using SlackNet;
using SlackNet.Events;

internal class WorkflowStepHandler : IEventHandler<FunctionExecuted>
{
    private static readonly Dictionary<string /*stepCallbackId*/, IWorkflowStep> Handlers = new();
    private readonly ISlackApiClient slack;

    public WorkflowStepHandler(ISlackApiClient slack) => this.slack = slack;

    public static void Initialize()
    {
        var handlerTypes = Assembly.GetExecutingAssembly()
           .GetTypes()
           .Where(Filter);

        foreach (var type in handlerTypes)
        {
            var handler = (IWorkflowStep)type.CreateInstance();

            Handlers.Add(handler.StepCallbackId, handler);
        }

        static bool Filter(Type type)
        {
            return type.GetInterface(nameof(IWorkflowStep)) != null;
        }
    }

    public async Task Handle(FunctionExecuted slackEvent)
    {
        if (Handlers.TryGetValue(slackEvent.Function.CallbackId, out var workflowStep) == false)
        {
            // note: 다른 봇의 이벤트일 수 있기 때문에 실패처리 하지 않습니다.
            return;
        }

        await workflowStep.OnRecv(this.slack, slackEvent);
    }
}