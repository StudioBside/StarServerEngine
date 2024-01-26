namespace SlackAssist.SlackNetHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Cs.Dynamic;
    using SlackAssist.Fremawork.Slack;
    using SlackNet;
    using SlackNet.Blocks;
    using SlackNet.Interaction;
    using SlackNet.WebApi;

    internal sealed class SlashCommandHandler : ISlashCommandHandler
    {
        public const string DefaultCommand = "/slackassist";
        private static readonly Dictionary<string, ISlashSubCommand> SubCommands = new();
        private readonly ISlackApiClient slack;
        public SlashCommandHandler(ISlackApiClient slack) => this.slack = slack;

        public static bool UseDefaultCommand { get; private set; } = false;

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

                foreach (var literal in subCommand.CommandLiterals)
                {
                    SubCommands.Add(literal.ToLowerInvariant(), subCommand);
                }
            }

            if (string.IsNullOrEmpty(commandPrefix))
            {
                UseDefaultCommand = true;
            }

            slackServices.RegisterSlashCommandHandler(DefaultCommand, instance);

            if (UseDefaultCommand == false)
            {
                foreach (var mainCommand in SubCommands.Values.GroupBy(e => e.Category.GetMainCommand()))
                {
                    slackServices.RegisterSlashCommandHandler(mainCommand.Key, instance);
                }
            }

            static bool Filter(Type type)
            {
                return type.GetInterface(nameof(ISlashSubCommand)) != null &&
                    type.IsAbstract == false;
            }
        }

        public static bool TryGetSubCommand(string literal, [MaybeNullWhen(false)] out ISlashSubCommand subCommand)
        {
            return SubCommands.TryGetValue(literal, out subCommand);
        }

        public static IEnumerable<Block> GetIntroduceBlocks()
        {
            foreach (var subCommandGroup in SubCommands.Values.GroupBy(e => e.Category))
            {
                yield return new HeaderBlock
                {
                    Text = subCommandGroup.Key.GetMainCommand(),
                };

                foreach (var subCommand in subCommandGroup)
                {
                    yield return subCommand.GetIntroduceBlock();
                    yield return new DividerBlock();
                }
            }
        }

        public async Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            SlashCommandResponse FailFactory(string additionalMessage) => new SlashCommandResponse
            {
                Message = new Message { Text = $"`{command.Command} {command.Text}` : 올바르지 않은 명령입니다. {additionalMessage}" },
            };

            Console.WriteLine($"user:@{command.UserName} channel:{command.ChannelName} command:{command.Text}");

            var tokens = command.Text.Split(' ');
            if (string.IsNullOrEmpty(command.Text) || tokens.Length < 1)
            {
                return FailFactory(additionalMessage: string.Empty);
            }

            (string subLiteral, string[] arguments) = (tokens[0], tokens[1..]);
            if (SubCommands.TryGetValue(subLiteral.ToLowerInvariant(), out var subCommand) == false)
            {
                return FailFactory(additionalMessage: string.Empty);
            }
            else if (command.Command.Contains(subCommand.Category.ToString().ToLowerInvariant()) == false)
            {
                return FailFactory($"`{subCommand.Category.GetMainCommand()} {command.Text}` 명령을 사용하세요.");
            }

            return new SlashCommandResponse
            {
                Message = await subCommand.Process(this.slack, command, arguments),
                ResponseType = ResponseType.Ephemeral,
            };
        }
    }
}