namespace Cs.Repl;

using System.Threading.Tasks;

public abstract class ReplHandlerBase
{
    protected ReplConsole Console { get; private set; } = null!;
    
    public abstract Task<string> Evaluate(string input);
    
    internal void SetConsole(ReplConsole console)
    {
        this.Console = console;
    }

    [ReplCommand(Name = "help", Description = "Displays help for the given command.")]
    internal string DumpHelp(string argument)
    {
        return "No help available.";
    }
}
