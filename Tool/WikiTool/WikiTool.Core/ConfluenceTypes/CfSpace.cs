namespace WikiTool.Core.ConfluenceTypes;

using System.Text;
using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;
using Newtonsoft.Json.Linq;

public sealed class CfSpace
{
    private readonly CfSpaceBulk bulk;
    private readonly List<CfPageBulk> pages = new();

    public CfSpace(CfSpaceBulk bulk)
    {
        this.bulk = bulk;
    }
    
    public int Id => this.bulk.Id;
    public string Name => this.bulk.Name;
    public IReadOnlyList<CfPageBulk> Pages => this.pages;

    public static async Task<CfSpace?> CreateAsync(RestApiClient client, CfSpaceBulk bulk)
    {
        var space = new CfSpace(bulk);
        if (await space.InitializeAsync(client) == false)
        {
            return null;
        }

        return space;
    }
    
    //// -----------------------------------------------------------------------------------------
    
    private static List<CfPageBulk>? JsonToPages(JObject obj)
    {
        List<CfPageBulk> pages = new();
        if (obj.TryGetArray("results", in pages, CfPageBulk.LoadFromJson) == false)
        {
            Log.Error($"Failed to get pages from: {obj}");
            return null;
        }
        
        return pages;
    }

    private async Task<bool> InitializeAsync(RestApiClient apiClient)
    {
        // cache pages
        var request = new HttpRequestMessage(HttpMethod.Get, $"wiki/api/v2/spaces/{this.Id}/pages");
        var response = await apiClient.SendAsync(request);
        var pages = await response.GetContentAs(JsonToPages);
        if (pages is null)
        {
            Log.Error($"Failed to get pages for space: {this.bulk.Key}");
            return false;
        }

        Log.Info($"Got {pages.Count} pages for space: {this.bulk.Name}");
        this.pages.AddRange(pages);
        
        foreach (var page in this.pages.OrderBy(e => e.Title))
        {
            Log.Info($" - id:{page.Id} title:{page.Title}");
        }
        
        return true;
    }
}
