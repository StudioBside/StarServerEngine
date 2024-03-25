namespace WikiTool.Core;

using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;
using WikiTool.Core.ConfluenceTypes;

public sealed class WikiToolCore
{
    private readonly WikiToolConfig config;
    private readonly RestApiClient client;
    private readonly List<CfSpaceBulk> spaces = new();
    private readonly WikiJsController wikiJs;

    public WikiToolCore()
    {
        this.config = JsonUtil.Load<WikiToolConfig>("wikitool.config.json");
        this.client = new RestApiClient(this.config.Confluence.Url);
        this.client.SetBasicAutohrization(this.config.Confluence.Username, this.config.Confluence.Password);
        
        this.wikiJs = WikiJsController.Instance;
        this.wikiJs.Initialize(this.config.WikiJsBackupPath);
        
        Log.Info($"wiki url:{this.config.Confluence.Url}");
    }
    
    public IReadOnlyList<CfSpaceBulk> Spaces => this.spaces;
    public CfSpace? CurrentSpace { get; private set; }
    
    public async Task<bool> InitializeAsync()
    {
        // cache spaces
        var request = new HttpRequestMessage(HttpMethod.Get, "wiki/api/v2/spaces?type=global&limit=100");
        var response = await this.client.SendAsync(request);
        var spaces = await response.GetContentAs(obj => obj["results"]!.ToObject<List<CfSpaceBulk>>());
        if (spaces is null)
        {
            Log.Error("Failed to get spaces.");
            return false;
        }

        Log.Info($"Got {spaces.Count} spaces.");
        this.spaces.AddRange(spaces);
        
        foreach (var space in this.spaces.OrderBy(e => e.Name))
        {
            Log.Info($" - id:{space.Id} key:{space.Key} name:{space.Name}");
        }

        return true;
    }
    
    public async Task<bool> SetSpaceById(int id)
    {
        var space = this.spaces.FirstOrDefault(e => e.Id == id);
        if (space is null)
        {
            Log.Error($"Space not found: {id}");
            return false;
        }
        
        if (this.CurrentSpace is not null &&
            this.CurrentSpace.Id == space.Id)
        {
            Log.Info($"Space is already set to {space.Name}.");
            return false;
        }

        var newSpace = await CfSpace.CreateAsync(this.client, space);
        if (newSpace is null)
        {
            Log.Error($"Failed to create space: {space.Name}");
            return false;
        }

        var prevName = this.CurrentSpace?.Name ?? "No space";
        this.CurrentSpace = newSpace;
        Log.Info($"Set space:{prevName} -> {space.Name}");
        return true;
    }
    
    public async Task<string> ConvertPages(int convertCount)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        foreach (var wjPage in this.wikiJs.Pages.Take(convertCount))
        {
            await this.CurrentSpace.GuaranteePage(this.client, wjPage);
        }
        
        return "Success";
    }
}
