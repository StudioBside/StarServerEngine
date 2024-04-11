namespace WikiTool.Core.ConfluenceTypes;

using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using WikiTool.Core.Transform;

public sealed class CfSpace
{
    private readonly CfSpaceBulk bulk;
    private readonly Dictionary<int, CfPage> pagesById = new();
    private CfPage rootPage = null!;

    public CfSpace(CfSpaceBulk bulk)
    {
        this.bulk = bulk;
    }
    
    public int Id => this.bulk.Id;
    public string Name => this.bulk.Name;
    public IEnumerable<CfPage> Pages => this.pagesById.Values;

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
        foreach (var page in this.pagesById.Values.Where(e => e.Parent is null))
        {
            sb.AppendLine(page.ToString());
        }
        
        return sb.ToString();
    }

    public async Task<bool> UploadPage(RestApiClient apiClient, WjPage wjPage, bool force)
    {
        var converter = ContentsConverter.Instance;

        // path에 해당하는 중간 페이지도 없다면 생성해 주어야 한다.
        CfPage parent = this.rootPage;
        var pathTokens = wjPage.Path.Split('/');
        for (int i = 0; i < pathTokens.Length - 1; i++)
        {
            var pathToken = pathTokens[i];
            var contents = converter.GetPathPageContents(pathToken);

            if (parent.TryGetSubPage(pathToken, out var page) == false)
            {
                page = await CfPage.CreateAsync(apiClient, this.Id, parent, pathToken, representation: "wiki", contents);
                if (page is null)
                {
                    Log.Error($"Failed to create page: {pathToken}");
                    return false;
                }
                
                Log.Info($"Create page: {pathToken}");
                CfPage.SetRelation(parent, page);
            }
            else if (converter.IsLatestPathPage(page.Body) == false)
            {
                if (await page.UpdateAsync(apiClient, "wiki", contents) == false)
                {
                    Log.Error($"Failed to update page: {pathToken}");
                }

                Log.Info($"Update page: {pathToken}");
            }
            
            parent = page;
        }

        var title = $"{wjPage.Title} ({wjPage.Id})";
        var content = converter.GetNodePageContents(wjPage.Content);
        if (parent.TryGetSubPage(title, out var prevPage))
        {
            if (force == false && converter.IsLatestNodePage(prevPage.Body) != false)
            {
                Log.Info($"up-to-date: {title}");
            }
            else
            {
                if (await prevPage.UpdateAsync(apiClient, "wiki", content) == false)
                {
                    Log.Error($"Failed to update page. title:{title} contentType:{wjPage.ContentType}");
                    Log.Debug($"content:{content}");
                    return false;
                }

                Log.Info($"Update page: {title}");
            }

            return true;
        }

        var newPage = await CfPage.CreateAsync(apiClient, this.Id, parent, title, "wiki", content); 
        if (newPage is null)
        {
            Log.Error($"Failed to create page: {wjPage.Path} conentType:{wjPage.ContentType}");
            return false;
        }
        
        Log.Debug($"Created page: {wjPage.Path}");
        CfPage.SetRelation(parent, newPage);
        return true;
    }

    public bool TryGetPage(string title, [MaybeNullWhen(false)] out CfPage page)
    {
        page = this.pagesById.Values.FirstOrDefault(e => e.Title == title);
        return page is not null;
    }
    
    public bool TryGetPage(int id, [MaybeNullWhen(false)] out CfPage page)
    {
        return this.pagesById.TryGetValue(id, out page);
    }
    
    public async Task<bool> DeletePage(RestApiClient apiClient, CfPage page)
    {
        if (page.Parent is null)
        {
            Log.Error($"Root page cannot be deleted: {page.Title}");
            return false;
        }

        if (await page.DeleteAsync(apiClient) == false)
        {
            Log.Error($"Failed to delete page: {page.Title}");
            return false;
        }

        int prevCount = this.pagesById.Count;
        this.pagesById.Remove(page.Id);
        Log.Info($"Deleted page: {page.Title} pageCount:{prevCount} -> {this.pagesById.Count}");
        return true;
    }
    
    public Task<string> ViewPage(RestApiClient apiClient, string title)
    {
        if (this.TryGetPage(title, out var page) == false)
        {
            Log.Error($"Page not found: {title}");
            return Task.FromResult(string.Empty);
        }

        return page.ViewAsync(apiClient);
    }
    
    //// -----------------------------------------------------------------------------------------

    private async Task<bool> InitializeAsync(RestApiClient apiClient)
    {
        string? url = $"wiki/api/v2/spaces/{this.Id}/pages?body-format=storage&expand=body.storage";

        while (string.IsNullOrEmpty(url) == false)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await apiClient.SendAsync(request);
            var bulkPages = await response.GetContentAs(JsonToPages);
            if (bulkPages is null)
            {
                Log.Error($"Failed to get pages for space: {this.bulk.Key}");
                return false;
            }
            
            foreach (var bulkPage in bulkPages)
            {
                var page = new CfPage(bulkPage);
                this.pagesById.Add(page.Id, page);
                if (bulkPage.ParentId == 0)
                {
                    if (this.rootPage is not null)
                    {
                        Log.Error($"Root page already exists: {this.rootPage.Title}");
                    }
                    else
                    {
                        this.rootPage = page;
                    }
                }
            }

            Log.Info($"Got {bulkPages.Count} pages for space: {this.bulk.Name}");
            
            // 다음 페이지가 있는지 확인한다.
            url = GetNextPageUrl(response.Headers);
        }

        foreach (var page in this.pagesById.Values)
        {
            page.Join(this.pagesById);
        }
        
        if (this.rootPage is null)
        {
            Log.Error($"Root page not found for space: {this.bulk.Name}");
            return false;
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
        
        static string? GetNextPageUrl(HttpResponseHeaders headers)
        {
            if (headers.TryGetValues("Link", out var values) == false ||
                !(values.FirstOrDefault() is { } linkValue))
            {
                return null;
            }

            /*
            </wiki/api/v2/spaces/163856/pages?cursor=eyJpZCI6IjM5OTc2OTciLCJjb250ZW50T3JkZXIiOiJpZCIsImNvbnRlbnRPcmRlclZhbHVlIjozOTk3Njk3fQ==>; rel="next",
             <https://starsavior.atlassian.net/wiki>; rel="base"
            */
            foreach (var token in linkValue.Split(','))
            {
                var tokens = token.Split(';');
                if (tokens.Length != 2)
                {
                    Log.Error($"Invalid link header: {linkValue}");
                    continue;
                }

                var relToken = tokens[1].Trim();
                if (relToken != "rel=\"next\"")
                {
                    continue;
                }

                var urlToken = tokens[0].Trim();
                if (urlToken[0] == '<' && urlToken[^1] == '>')
                {
                    return urlToken[1..^1];
                }
            }

            return null;
        }
    }
}
