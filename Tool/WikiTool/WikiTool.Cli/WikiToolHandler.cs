namespace WikiTool.Cli;

using System.Text;
using System.Threading.Tasks;
using Cs.Repl;
using WikiTool.Core;

public sealed class WikiToolHandler : ReplHandlerBase
{
    private readonly WikiToolCore tool;

    public WikiToolHandler()
    {
        this.tool = new WikiToolCore();
    }
    
    public override Task<bool> InitializeAsync()
    {
        return this.tool.InitializeAsync();
    }
    
    public override string GetPrompt()
    {
        return this.tool.CurrentSpace?.Name ?? "No space";
    }
    
    [ReplCommand(Name = "spaces", Description = "Gets the list of spaces.")]
    public string GetSpaces(string argument)
    {
        var sb = new StringBuilder();
        foreach (var space in this.tool.Spaces.OrderBy(e => e.Name))
        {
            sb.AppendLine($"id:{space.Id} key:{space.Key} name:{space.Name}");
        }
        
        return sb.ToString();
    }

    [ReplCommand(Name = "set-space", Description = "Sets the space to the given key.")]
    public async Task<string> SetSpace(string argument)
    {
        if (int.TryParse(argument, out int spaceId) == false)
        {
            return $"Invalid space id: {argument}";
        }

        if (await this.tool.SetSpaceById(spaceId) == false)
        {
            return $"Space not found: {argument}";
        }
        
        return string.Empty;
    }
}
