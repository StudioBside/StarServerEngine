namespace Cs.Repl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cs.Dynamic;
using Cs.Logging;
using Cs.Repl.Detail;

public sealed class ReplConsole
{
    private readonly Dictionary<string, Command> commands = new(StringComparer.CurrentCultureIgnoreCase);
    
    public ReplHandlerBase Handler { get; private set; } = null!;
    internal IEnumerable<Command> Commands => this.commands.Values;

    public async Task<bool> InitializeAsync(ReplHandlerBase handler)
    {
        if (await handler.InitializeAsync() == false)
        {
            return false;
        }

        this.Handler = handler;
        this.Handler.SetConsole(this);
        
        var handlerType = handler.GetType();
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        {
            foreach (var method in handlerType.GetMethods(bindingFlags))
            {
                var attr = method.GetCustomAttribute<ReplCommandAttribute>();
                if (attr is not null)
                {
                    var rawHandler = DelegateFactory<Command.RawHandlerType>.CreateAwaitableMemberFunction(method);
                    var command = new Command(attr.Name, attr.Description, rawHandler);
                    this.commands.Add(command.Name, command);
                }
            }
        }
        
        Log.Info($"Initialized {this.commands.Count} commands.");
        return true;
    }

    public async Task ExecuteBatch(IEnumerable<string> commands)
    {
        foreach (var command in commands)
        {
            Console.WriteLine($"{this.Handler.GetPrompt()}> {command}");
            var result = await this.ProcessSingleCommand(command);

            Console.WriteLine();
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }

    public async Task StartLoop()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine($"Type 'exit' to end.");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{this.Handler.GetPrompt()}> ");

            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine() ?? string.Empty;

            if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                break;
            }
            
            var result = await this.ProcessSingleCommand(input);

            Console.WriteLine();
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }

    public Task<string> ExecuteDirect(string command)
    {
        return this.ProcessSingleCommand(command);
    }

    public bool HasCommand(string command)
    {
        return this.commands.Keys.Any(command.StartsWith);
    }

    public void DumpHelpText(StringBuilder sb, string prefix)
    {
        foreach (var command in this.commands.Values)
        {
            if (command.Name == "help")
            {
                // 밖에서 help 메세지를 직접 뽑을 때는 제외
                continue;
            }

            sb.AppendLine($"{prefix}{command.Name} : {command.Description}");
        }
    }

    //// -----------------------------------------------------------------------------------------

    private async Task<string> ProcessSingleCommand(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        
        // Split the input into command and argument
        string command;
        string argument;
        int spaceIndex = input.IndexOf(' ');
        if (spaceIndex == -1)
        {
            command = input;
            argument = string.Empty;
        }
        else
        {
            command = input[..spaceIndex];
            argument = input[(spaceIndex + 1)..];
        }
        
        try
        {
            string result = string.Empty;
            if (this.commands.TryGetValue(command, out var cmd))
            {
                return await cmd.Invoke(this.Handler, argument);
            }

            return await this.Handler.Evaluate(input);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
