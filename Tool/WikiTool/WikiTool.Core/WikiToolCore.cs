namespace WikiTool.Core;

using System.Net;
using System.Text;
using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;
using Microsoft.VisualBasic;
using WikiTool.Core.ConfluenceTypes;

public sealed class WikiToolCore
{
    private readonly WikiToolConfig config;
    private readonly RestApiClient client;
    private readonly List<CfSpaceBulk> spaces = new();
    private readonly WikiJsController wikiJs;

    public WikiToolCore()
    {
        this.config = JsonUtil.Load<WikiToolConfig>("wikitool.config.json");
        this.client = new RestApiClient(this.config.Confluence.Url);
        this.client.SetBasicAutohrization(this.config.Confluence.Username, this.config.Confluence.Password);
        
        this.wikiJs = WikiJsController.Instance;
        this.wikiJs.Initialize(this.config.WikiJsBackupPath);
        
        Log.Info($"wiki url:{this.config.Confluence.Url}");
    }
    
    public WikiToolConfig Config => this.config;
    public IReadOnlyList<CfSpaceBulk> Spaces => this.spaces;
    public CfSpace? CurrentSpace { get; private set; }
    
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

        return true;
    }
    
    public async Task<bool> SetSpaceById(int id)
    {
        var space = this.spaces.FirstOrDefault(e => e.Id == id);
        if (space is null)
        {
            Log.Error($"Space not found: {id}");
            return false;
        }
        
        if (this.CurrentSpace is not null &&
            this.CurrentSpace.Id == space.Id)
        {
            Log.Info($"Space is already set to {space.Name}.");
            return false;
        }

        var newSpace = await CfSpace.CreateAsync(this.client, space);
        if (newSpace is null)
        {
            Log.Error($"Failed to create space: {space.Name}");
            return false;
        }

        var prevName = this.CurrentSpace?.Name ?? "No space";
        this.CurrentSpace = newSpace;
        Log.Info($"Set space:{prevName} -> {space.Name}");
        return true;
    }
    
    public async Task<string> ConvertPages(int convertCount)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        int success = 0;
        int failed = 0;
        foreach (var wjPage in this.wikiJs.Pages.Take(convertCount))
        {
            var result = await this.CurrentSpace.GuaranteePage(this.client, wjPage);
            if (result)
            {
                success++;
            }
            else
            {
                failed++;
            }
            
            Log.Info($"success:{success} failed:{failed}");
        }
        
        return "finished";
    }
    
    public async Task<string> ConvertById(int pageId)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        var wjPage = this.wikiJs.Pages.FirstOrDefault(e => e.Id == pageId);
        if (wjPage is null)
        {
            return $"Page not found: {pageId}";
        }

        var result = await this.CurrentSpace.GuaranteePage(this.client, wjPage);
        return $"pageId:{pageId} title:{wjPage.Title} result:{result}";
    }
    
    public Task<string> TestPage(int percent)
    {
        if (this.CurrentSpace is null)
        {
            return Task.FromResult("선택된 space가 없습니다.");
        }
        
        var wjPage = this.wikiJs.Pages[0];
        int contentLength = wjPage.Render.Length * percent / 100;
        Log.Info($"contentLength:{wjPage.Render.Length} -> {contentLength}");
        var content = wjPage.Render[..contentLength];
        return this.GuaranteePage(wjPage.Title, content);
    }
    
    public string ListPages()
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        var sb = new StringBuilder();
        foreach (var page in this.CurrentSpace.Pages)
        {
            sb.AppendLine($"id:{page.Id} title:{page.Title}");
        }
        
        return sb.ToString();
    }
    
    public Task<string> ViewPage(int cfPageId)
    {
        if (this.CurrentSpace is null)
        {
            return Task.FromResult("선택된 space가 없습니다.");
        }
        
        if (this.CurrentSpace.TryGetPage(cfPageId, out var page) == false)
        {
            return Task.FromResult($"Page not found: {cfPageId}");
        }
        
        return page.ViewAsync(this.client);
    }

    public string SearchPage(string keyword)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        var sb = new StringBuilder();
        foreach (var page in this.CurrentSpace.Pages.Where(e => e.Title.Contains(keyword)))
        {
            sb.AppendLine($"id:{page.Id} title:{page.Title}");
        }
        
        return sb.ToString();
    }
    
    public async Task<string> CleanGarbages()
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        // 제목에 id를 붙이지 않고 만들어진 페이지들을 삭제한다.
        foreach (var wjPage in this.wikiJs.Pages)
        {
            if (this.CurrentSpace.TryGetPage(wjPage.Title, out var cfPage) == false)
            {
                continue;
            }

            await this.CurrentSpace.DeletePage(this.client, cfPage);
        }
        
        return "Success";
    }
    
    public async Task<string> GuaranteePage(string title, string content)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        var wjPage = new WjPage
        {
            Id = 0,
            Path = title,
            Title = title,
            Description = string.Empty,
            Content = content,
            Render = WebUtility.HtmlEncode(content),
            Toc = string.Empty,
            ContentType = "html",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Author = new WjUser { Id = 0, Name = "WikiTool", Email = string.Empty },
            Creator = new WjUser { Id = 0, Name = "WikiTool", Email = string.Empty },
        };
        
        if (await this.CurrentSpace.GuaranteePage(this.client, wjPage))
        {
            return "Success";
        }

        return "Failed";
    }
}
