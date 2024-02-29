namespace WikiTool.Core;

using Cs.Core.Util;
using Cs.HttpClient;

public sealed class WikiToolCore
{
    private readonly WikiToolConfig config;
    private readonly RestApiClient client;

    public WikiToolCore()
    {
        this.config = JsonUtil.Load<WikiToolConfig>("wikitool.config.json");
        this.client = new RestApiClient(this.config.Confluence.Url);
    }
    
    public async Task<string> GetSpaces()
    {
        var response = await this.client.GetStringAsync("/rest/api/space?type=global&limit=1000");
        return response;
    }
}
