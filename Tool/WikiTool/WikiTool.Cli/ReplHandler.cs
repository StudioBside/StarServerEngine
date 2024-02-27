namespace WikiTool.Cli;

using System.Threading.Tasks;
using Cs.Repl;

public sealed class ReplHandler : ReplHandlerBase
{
    [ReplCommand(Name = "hello", Description = "Says hello to the given name.")]
    public string Hello(string argument)
    {
        return $"Hello, {argument}!";
    }
}
