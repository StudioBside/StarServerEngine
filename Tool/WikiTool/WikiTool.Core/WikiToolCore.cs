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

    public WikiToolCore()
    {
        this.config = JsonUtil.Load<WikiToolConfig>("wikitool.config.json");
        this.client = new RestApiClient(this.config.Confluence.Url);
        this.client.SetBasicAutohrization(this.config.Confluence.Username, this.config.Confluence.Password);
        
        Log.Info($"wiki url:{this.config.Confluence.Url}");
    }
    
    public IReadOnlyList<CfSpaceBulk> Spaces => this.spaces;
    public CfSpaceBulk? CurrentSpace { get; private set; }
    
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

        this.CurrentSpace = this.spaces.FirstOrDefault();
        return true;
    }
    
    public bool SetSpaceById(int id)
    {
        var space = this.spaces.FirstOrDefault(e => e.Id == id);
        if (space is null)
        {
            Log.Error($"Space not found: {id}");
            return false;
        }
        
        if (this.CurrentSpace == space)
        {
            Log.Info($"Space is already set to {space.Name}.");
            return false;
        }

        var prevName = this.CurrentSpace?.Name ?? "No space";
        this.CurrentSpace = space;
        Log.Info($"Set space:{prevName} -> {space.Name}.");
        return true;
    }
}
