namespace WikiTool.Core.ConfluenceTypes;

using System.Text;
using Cs.HttpClient;
using Cs.Logging;
using Newtonsoft.Json;

public sealed class CfPage
{
    private readonly CfPageBulk bulk;
    private readonly List<CfPage> children = new();
    
    public CfPage(CfPageBulk bulk)
    {
        this.bulk = bulk;
    }
    
    public int Id => this.bulk.Id;
    public string Title => this.bulk.Title;
    public CfPage? Parent { get; private set; }
    
    public static void SetRelation(CfPage parent, CfPage child)
    {
        child.Parent = parent;
        parent.children.Add(child);
    }
    
    public static async Task<CfPage?> CreateAsync(
        RestApiClient apiClient,
        int spaceId,
        CfPage? parent,
        string title,
        string body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"wiki/api/v2/pages");
        string bodyContent = JsonConvert.SerializeObject(new
        {
            spaceId = spaceId,
            status = "current",
            title = title,
            parentId = parent?.Id,
            body = new
            {
                representation = "storage",
                value = body,
            },
        });

        request.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
        
        var response = await apiClient.SendAsync(request);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            Log.Error($"Failed to create page: {title} statusCode:{response.StatusCode}");
            return null;
        }

        var bulkPage = await response.GetContentAs<CfPageBulk>();
        if (bulkPage is null)
        {
            Log.Error($"Failed to create page: {title}");
            return null;
        }

        return new CfPage(bulkPage!);
    }

    public override string ToString()
    {
        return this.DumpToString(indent: 0);
    }
    
    public void Join(IReadOnlyDictionary<int, CfPage> pages)
    {
        if (this.bulk.ParentId == 0)
        {
            // 부모가 없다면 조인할 일이 없다.
            return;
        }

        if (pages.TryGetValue(this.bulk.ParentId, out var parent) == false)
        {
            Log.Error($"Parent page not found: {this.bulk.ParentId}");
            return;
        }

        SetRelation(parent, this);
    }
    
    //// -------------------------------------------------------------------------------------
    
    private string DumpToString(int indent)
    {
        var space = string.Empty.PadRight(indent * 2, ' ');
        var sb = new StringBuilder();
        sb.AppendLine($"{space} - id:{this.Id} title:{this.Title} (parentId:{this.bulk.ParentId})");
        foreach (var child in this.children)
        {
            sb.AppendLine(child.DumpToString(indent + 1));
        }
        
        return sb.ToString();
    }
}
