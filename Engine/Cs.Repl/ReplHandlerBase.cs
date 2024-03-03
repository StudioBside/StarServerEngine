namespace Cs.Repl;

using System;
using System.Text;
using System.Threading.Tasks;

public abstract class ReplHandlerBase
{
    protected ReplConsole Console { get; private set; } = null!;
    
    public virtual Task<bool> InitializeAsync()
    {
        return Task.FromResult(true);
    }
    
    public virtual string GetPrompt()
    {
        return "REPL";
    }
    
    public virtual Task<string> Evaluate(string input)
    {
        var result = $"Unknown command: {input}";
        return Task.FromResult(result);
    }
    
    internal void SetConsole(ReplConsole console)
    {
        this.Console = console;
    }

    [ReplCommand(Name = "help", Description = "Displays help for the given command.")]
    internal string DumpHelp(string argument)
    {
        var sb = new StringBuilder();
        foreach (var command in this.Console.Commands)
        {
            if (string.IsNullOrEmpty(argument) || command.Name.Equals(argument, StringComparison.CurrentCultureIgnoreCase))
            {
                sb.AppendLine($"{command.Name} - {command.Description}");
            }
        }
        
        return sb.ToString();
    }
}
