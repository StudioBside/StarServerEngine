namespace SlackAssist.SlackNetHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Cs.Dynamic;
    using Cs.Logging;
    using SlackAssist.Configs;
    using SlackAssist.Fremawork.Slack;
    using SlackNet;
    using SlackNet.Blocks;
    using SlackNet.Interaction;
    using SlackNet.WebApi;

    internal sealed class SlashCommandHandler : ISlashCommandHandler
    {
        private static readonly Dictionary<string, SlashCommandHolder> Commands = new();
        private readonly ISlackApiClient slack;
        public SlashCommandHandler(ISlackApiClient slack) => this.slack = slack;

        public static string BuildCommand(string command)
        {
            var commandPrefix = SlackAssistConfig.Instance.Slack.SlashCommandPrefix;
            return $"/{commandPrefix}{command}".ToLowerInvariant();
        }

        public static void Initialize(SlackServiceBuilder slackServices, ISlackApiClient slack, string commandPrefix)
        {
            var instance = new SlashCommandHandler(slack);
            var subCommandTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(Filter);

            foreach (var type in subCommandTypes)
            {
                if (type.CreateInstance() is not ISlashSubCommand subCommand)
                {
                    continue;
                }

                var command = subCommand.Command.ToLowerInvariant();
                if (Commands.TryGetValue(command, out var holder) == false)
                {
                    holder = new SlashCommandHolder();
                    Commands.Add(command, holder);
                    slackServices.RegisterSlashCommandHandler(command, instance);
                }

                holder.Add(subCommand);
            }

            static bool Filter(Type type)
            {
                return type.GetInterface(nameof(ISlashSubCommand)) != null &&
                    type.IsAbstract == false;
            }
        }

        public static bool TryGetCommand(string command, string literal, [MaybeNullWhen(false)] out ISlashSubCommand subCommand)
        {
            if (Commands.TryGetValue(command.ToLowerInvariant(), out var holder))
            {
                return holder.TryGetValue(literal, out subCommand);
            }

            subCommand = null;
            return false;
        }

        public static IEnumerable<Block> GetIntroduceBlocks()
        {
            foreach (var command in Commands)
            {
                yield return new HeaderBlock
                {
                    Text = command.Key,
                };

                foreach (var subCommand in command.Value.SubCommands)
                {
                    yield return subCommand.GetIntroduceBlock();
                    yield return new DividerBlock();
                }
            }
        }

        public async Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            SlashCommandResponse FailFactory() => new SlashCommandResponse
            {
                Message = new Message { Text = $"`{command.Command} {command.Text}` : 올바르지 않은 명령입니다." },
            };

            Log.Debug($"user:@{command.UserName} channel:{command.ChannelName} command:{command.Text}");

            var tokens = command.Text.Split(' ');
            if (string.IsNullOrEmpty(command.Text) || tokens.Length < 1)
            {
                return FailFactory();
            }

            (string subLiteral, string[] arguments) = (tokens[0], tokens[1..]);
            if (TryGetCommand(command.Command, subLiteral, out var subCommand) == false)
            {
                return FailFactory();
            }

            return new SlashCommandResponse
            {
                Message = await subCommand.Process(this.slack, command, arguments),
                ResponseType = ResponseType.Ephemeral,
            };
        }

        private sealed record SlashCommandHolder
        {
            private readonly List<ISlashSubCommand> subCommands = new();
            private readonly Dictionary<string, ISlashSubCommand> subCommandLiterals = new();

            public IEnumerable<ISlashSubCommand> SubCommands => this.subCommands;

            public bool TryGetValue(string literal, [MaybeNullWhen(false)] out ISlashSubCommand subCommand) => this.subCommandLiterals.TryGetValue(literal.ToLowerInvariant(), out subCommand);

            public void Add(ISlashSubCommand subCommand)
            {
                this.subCommands.Add(subCommand);
                foreach (var literal in subCommand.CommandLiterals)
                {
                    this.subCommandLiterals.Add(literal.ToLowerInvariant(), subCommand);
                }
            }
        }
    }
}