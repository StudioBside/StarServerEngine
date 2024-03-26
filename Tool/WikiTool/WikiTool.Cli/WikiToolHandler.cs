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
    
    public WikiToolConfig Config => this.tool.Config;
    
    public override Task<bool> InitializeAsync()
    {
        return this.tool.InitializeAsync();
    }
    
    public override string GetPrompt()
    {
        return this.tool.CurrentSpace?.Name ?? "No space";
    }
    
    [ReplCommand(Name = "spaces", Description = "컨플루언스 스페이스 목록을 출력합니다.")]
    public Task<string> GetSpaces(string argument)
    {
        var sb = new StringBuilder();
        foreach (var space in this.tool.Spaces.OrderBy(e => e.Name))
        {
            sb.AppendLine($"id:{space.Id} key:{space.Key} name:{space.Name}");
        }

        return Task.FromResult(sb.ToString());
    }

    [ReplCommand(Name = "set-space", Description = "입력받은 spaceId에 해당하는 스페이스를 선택합니다.")]
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

    [ReplCommand(Name = "view-space", Description = "현재 선택된 스페이스의 페이지 목록을 출력합니다.")]
    public async Task<string> ViewSpace(string argument)
    {
        if (this.tool.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        await Task.Delay(0);
        return this.tool.CurrentSpace.ToString();
    }

    [ReplCommand(Name = "convert-pages", Description = "현재 선택된 스페이스의 페이지 목록을 출력합니다.")]
    public Task<string> ConvertPages(string argument)
    {
        if (int.TryParse(argument, out int convertCount) == false)
        {
            return Task.FromResult($"변환할 페이지의 개수를 입력해야 합니다: {argument}");
        }
        
        return this.tool.ConvertPages(convertCount);
    }
}
