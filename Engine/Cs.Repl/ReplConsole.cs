namespace Cs.Repl;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cs.Logging;
using Cs.Repl.Detail;

public sealed class ReplConsole
{
    private readonly Dictionary<string, Command> commands = new(StringComparer.CurrentCultureIgnoreCase);
    
    public string Prompt { get; set; } = "REPL";
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
                    var command = new Command(attr.Name, attr.Description, method);
                    this.commands.Add(command.Name, command);
                }
            }
        }
        
        Log.Info($"Initialized {this.commands.Count} commands.");
        return true;
    }

    public async Task Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine($"Type 'exit' to end.");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{this.Prompt}> ");

            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine() ?? string.Empty;

            if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                break;
            }

            if (string.IsNullOrEmpty(input))
            {
                continue;
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
                    result = await cmd.Invoke(this.Handler, argument);
                }
                else
                {
                    result = await this.Handler.Evaluate(input);
                }

                Console.WriteLine();
                Console.WriteLine(result);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
