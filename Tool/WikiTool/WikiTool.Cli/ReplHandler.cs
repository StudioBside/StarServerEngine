namespace WikiTool.Cli;

using System.Threading.Tasks;
using Cs.Repl;
using WikiTool.Core;

public sealed class ReplHandler : ReplHandlerBase
{
    private readonly WikiToolCore tool;

    public ReplHandler()
    {
        this.tool = new WikiToolCore();
    }
    
    [ReplCommand(Name = "hello", Description = "Says hello to the given name.")]
    public string Hello(string argument)
    {
        return $"Hello, {argument}!";
    }
    
    [ReplCommand(Name = "spaces", Description = "Gets the list of spaces.")]
    public async Task<string> GetSpaces(string argument)
    {
        return await this.tool.GetSpaces();
    }
}
