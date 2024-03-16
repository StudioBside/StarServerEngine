namespace WikiTool.Core.ConfluenceTypes;

using System.Text;
using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;
using Newtonsoft.Json.Linq;

public sealed class CfSpace
{
    private readonly CfSpaceBulk bulk;
    private Dictionary<int, CfPage> pages = null!;

    public CfSpace(CfSpaceBulk bulk)
    {
        this.bulk = bulk;
    }
    
    public int Id => this.bulk.Id;
    public string Name => this.bulk.Name;
    public IEnumerable<CfPage> Pages => this.pages.Values;

    public static async Task<CfSpace?> CreateAsync(RestApiClient client, CfSpaceBulk bulk)
    {
        var space = new CfSpace(bulk);
        if (await space.InitializeAsync(client) == false)
        {
            return null;
        }

        return space;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var page in this.pages.Values.Where(e => e.Parent is null))
        {
            sb.AppendLine(page.ToString());
        }
        
        return sb.ToString();
    }

    //// -----------------------------------------------------------------------------------------

    private async Task<bool> InitializeAsync(RestApiClient apiClient)
    {
        // cache pages
        var request = new HttpRequestMessage(HttpMethod.Get, $"wiki/api/v2/spaces/{this.Id}/pages");
        var response = await apiClient.SendAsync(request);
        var bulkPages = await response.GetContentAs(JsonToPages);
        if (bulkPages is null)
        {
            Log.Error($"Failed to get pages for space: {this.bulk.Key}");
            return false;
        }

        Log.Info($"Got {bulkPages.Count} pages for space: {this.bulk.Name}");
        
        this.pages = bulkPages
            .Select(e => new CfPage(e))
            .ToDictionary(e => e.Id);

        foreach (var page in this.pages.Values)
        {
            page.Join(this.pages);
        }

        foreach (var page in this.pages.Values)
        {
            Log.Info(page.ToString());
        }
        
        return true;

        static List<CfPageBulk>? JsonToPages(JObject obj)
        {
            List<CfPageBulk> pages = new();
            if (obj.TryGetArray("results", in pages, CfPageBulk.LoadFromJson) == false)
            {
                Log.Error($"Failed to get pages from: {obj}");
                return null;
            }
            
            return pages;
        }
    }
}
