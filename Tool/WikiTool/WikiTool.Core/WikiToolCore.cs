namespace WikiTool.Core;

using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;

public sealed class WikiToolCore
{
    private readonly WikiToolConfig config;
    private readonly RestApiClient client;
    private readonly List<SpaceModel> spaces = new();

    public WikiToolCore()
    {
        this.config = JsonUtil.Load<WikiToolConfig>("wikitool.config.json");
        this.client = new RestApiClient(this.config.Confluence.Url);
        this.client.SetBasicAutohrization(this.config.Confluence.Username, this.config.Confluence.Password);
        
        Log.Info($"wiki url:{this.config.Confluence.Url}");
    }
    
    public IReadOnlyList<SpaceModel> Spaces => this.spaces;
    
    public async Task<bool> InitializeAsync()
    {
        // cache spaces
        var request = new HttpRequestMessage(HttpMethod.Get, "wiki/api/v2/spaces?type=global&limit=100");
        var response = await this.client.SendAsync(request);
        var spaces = await response.GetContentAs(obj => obj["results"]!.ToObject<List<SpaceModel>>());
        if (spaces is null)
        {
            Log.Error("Failed to get spaces.");
            return false;
        }

        Log.Info($"Got {spaces.Count} spaces.");
        this.spaces.AddRange(spaces);
        return true;
    }
}
