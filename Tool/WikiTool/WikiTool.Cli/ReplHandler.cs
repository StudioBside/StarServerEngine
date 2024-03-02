namespace WikiTool.Cli;

using System.Text;
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
    
    public override Task<bool> InitializeAsync()
    {
        return this.tool.InitializeAsync();
    }
    
    [ReplCommand(Name = "hello", Description = "Says hello to the given name.")]
    public string Hello(string argument)
    {
        return $"Hello, {argument}!";
    }
    
    [ReplCommand(Name = "spaces", Description = "Gets the list of spaces.")]
    public string GetSpaces(string argument)
    {
        var sb = new StringBuilder();
        foreach (var space in this.tool.Spaces)
        {
            sb.AppendLine($"id:{space.Id} key:{space.Key} name:{space.Name}");
        }
        
        return sb.ToString();
    }

    [ReplCommand(Name = "set-space", Description = "Sets the space to the given key.")]
    public string SetSpace(string argument)
    {
        var space = this.tool.Spaces.FirstOrDefault(e => e.Id.ToString() == argument);
        if (space is null)
        {
            return $"Space not found: {argument}";
        }

        this.Console.Prompt = space.Name;
        return $"Set space to {argument}.";
    }
}
