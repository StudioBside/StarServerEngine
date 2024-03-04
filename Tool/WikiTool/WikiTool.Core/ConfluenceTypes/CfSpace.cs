namespace WikiTool.Core.ConfluenceTypes;

using Cs.HttpClient;
using Cs.Logging;

public sealed class CfSpace
{
    private readonly CfSpaceBulk bulk;
    private readonly List<CfPageBulk> pages = new();

    public CfSpace(CfSpaceBulk bulk)
    {
        this.bulk = bulk;
    }
    
    public IReadOnlyList<CfPageBulk> Pages => this.pages;

    public static async Task<CfSpace> CreateAsync(RestApiClient client, CfSpaceBulk bulk)
    {
        var space = new CfSpace(bulk);
        await space.InitializeAsync(client);
        return space;
    }
    
    //// -----------------------------------------------------------------------------------------
    
    private async Task InitializeAsync(RestApiClient client)
    {
        // cache pages
        var request = new HttpRequestMessage(HttpMethod.Get, $"wiki/api/v2/spaces/{this.bulk.Key}/content?type=page&limit=100");
        var response = await client.SendAsync(request);
        var pages = await response.GetContentAs(obj => obj["results"]!.ToObject<List<CfPageBulk>>());
        if (pages is null)
        {
            Log.Error($"Failed to get pages for space: {this.bulk.Key}");
            return;
        }

        Log.Info($"Got {pages.Count} pages for space: {this.bulk.Key}");
        this.pages.AddRange(pages);
        
        foreach (var page in this.pages.OrderBy(e => e.Title))
        {
            Log.Info($" - id:{page.Id} title:{page.Title}");
        }
    }
}
