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
        this.client.SetBasicAutohrization(this.config.Confluence.Username, this.config.Confluence.Password);
    }
    
    public async Task<string> GetSpaces()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "wiki/api/v2/spaces?type=global&limit=100");
        var response = await this.client.SendAsync(request);
        return await response.GetRawContent();
    }
}
