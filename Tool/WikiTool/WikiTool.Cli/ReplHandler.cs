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
}
