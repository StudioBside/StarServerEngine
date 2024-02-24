namespace WikiTool.Cli;

using System.Threading.Tasks;
using Cs.Repl;

public sealed class ReplHandler : ReplHandlerBase
{
    public override async Task<string> Evaluate(string input)
    {
        await Task.Delay(0);
        return input;
    }
    
    [ReplCommand(Name = "hello", Description = "Says hello to the given name.")]
    public string Hello(string argument)
    {
        return $"Hello, {argument}!";
    }
}
