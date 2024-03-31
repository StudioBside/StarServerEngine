namespace WikiTool.Core.ConfluenceTypes;

using System.Diagnostics.CodeAnalysis;
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
    public string Body => this.bulk.Body.Value;
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

    public async Task<bool> UpdateAsync(RestApiClient apiClient, string body)
    {
        if (this.bulk.Body.Value.Equals(body))
        {
            Log.Info($"Page body is not changed: {this.Title}");
            return true;
        }
        
        var request = new HttpRequestMessage(HttpMethod.Put, $"wiki/api/v2/pages/{this.Id}");
        string bodyContent = JsonConvert.SerializeObject(new
        {
            id = this.Id,
            status = "current",
            title = this.Title,
            body = new
            {
                representation = "storage",
                value = body,
            },
            version = new
            {
                number = this.bulk.Version.Number + 1,
            },
        });

        request.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
        
        var response = await apiClient.SendAsync(request);
        if (response.IsSuccessStatusCode == false)
        {
            // Log.Error($"{this.Title} statusCode:{response.StatusCode}");
            return false;
        }

        var bulkPage = await response.GetContentAs<CfPageBulk>();
        if (bulkPage is null)
        {
            Log.Error($"Failed to update page: {this.Title}");
            return false;
        }
        
        bulkPage.Body.Representation = "storage";
        bulkPage.Body.Value = body;

        this.bulk.Update(bulkPage);
        return true;
    }
    
    public async Task<string> ViewAsync(RestApiClient apiClient)
    {
        if (string.IsNullOrEmpty(this.Body) == false)
        {
            return this.Body;
        }
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"wiki/api/v2/pages/{this.Id}?body-format=storage");
        var response = await apiClient.SendAsync(request);
        if (response.IsSuccessStatusCode == false)
        {
            Log.Error($"Failed to get page: {this.Title} statusCode:{response.StatusCode}");
            return string.Empty;
        }

        var bulkPage = await response.GetContentAs<CfPageBulk>();
        if (bulkPage is null)
        {
            Log.Error($"Failed to get page: {this.Title}");
            return string.Empty;
        }

        this.bulk.Update(bulkPage);
        return this.Body;
    }
    
    public async Task<bool> DeleteAsync(RestApiClient apiClient)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"wiki/api/v2/pages/{this.Id}");
        var response = await apiClient.SendAsync(request);
        if (response.IsSuccessStatusCode == false)
        {
            Log.Error($"Failed to delete page: {this.Title} statusCode:{response.StatusCode}");
            return false;
        }

        return true;
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
    
    public bool TryGetSubPage(string title, [MaybeNullWhen(false)] out CfPage page)
    {
        page = this.children.FirstOrDefault(e => e.Title == title);
        return page is not null;
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
