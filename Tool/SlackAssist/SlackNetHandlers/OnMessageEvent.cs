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

internal class OnMessageEvent : IEventHandler<MessageEvent>
{
    private static readonly List<IChatReaction> Handlers = new();
    private readonly ISlackApiClient slack;

    public OnMessageEvent(ISlackApiClient slack) => this.slack = slack;
    public static IEnumerable<IChatReaction> MessageHandlers => Handlers;

    public static void Initialize()
    {
        var handlerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(Filter);

        foreach (var type in handlerTypes)
        {
            var handler = type.CreateInstance() as IChatReaction;
            Handlers.Add(handler!);
        }

        static bool Filter(Type type)
        {
            return type.GetInterface(nameof(IChatReaction)) != null &&
                type.IsAbstract == false;
        }
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        foreach (var handler in Handlers)
        {
            var eventRecord = handler.IsTargetEvent(slackEvent);
            if (eventRecord is null)
            {
                continue;
            }

            await handler.Process(this.slack, eventRecord);
        }
    }

    //// ---------------------------------------------------------------------------------
}
