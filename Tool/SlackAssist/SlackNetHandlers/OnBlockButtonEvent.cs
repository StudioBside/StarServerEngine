namespace SlackAssist.Handlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cs.Dynamic;
using Cs.Logging;

using SlackAssist.Fremawork.Slack;

using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;

internal class OnBlockButtonEvent : IBlockActionHandler<ButtonAction>
{
    private static readonly Dictionary<string /*actionId*/, IButtonAction> Handlers = new();
    private readonly ISlackApiClient slack;

    public OnBlockButtonEvent(ISlackApiClient slack) => this.slack = slack;

    public static void Initialize()
    {
        var buttonTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(Filter);

        foreach (var type in buttonTypes)
        {
            if (type.CreateInstance() is not IButtonAction buttonAction)
            {
                continue;
            }

            Handlers.Add(buttonAction.ActionId, buttonAction);
        }

        static bool Filter(Type type)
        {
            return type.Name is not null &&
                type.GetInterface(nameof(IButtonAction)) != null &&
                type.IsAbstract == false;
        }
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (action.ActionId == "url")
        {
            return;
        }

        if (Handlers.TryGetValue(action.ActionId, out var handler) == false)
        {
            Log.Warn($"invalid action. actionId:{action.ActionId}");
            return;
        }

        await handler.Process(this.slack, action, request);
    }
}