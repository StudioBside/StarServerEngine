namespace WikiTool.Core;

using System.Net.Http.Headers;
using System.Text;
using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WikiTool.Core.ConfluenceTypes;
using WikiTool.Core.Transform;

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
    public WikiJsController WikiJs => this.wikiJs;

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
            var result = await this.CurrentSpace.UploadPage(this.client, wjPage, force: false);
            if (result)
            {
                success++;
            }
            else
            {
                failed++;
            }
        }
        
        Log.Info($"success:{success} failed:{failed}");
        return "finished";
    }
    
    public async Task<string> ConvertById(int pageId, bool force)
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

        var result = await this.CurrentSpace.UploadPage(this.client, wjPage, force);
        return $"pageId:{pageId} title:{wjPage.Title}";
    }
    
    public string PreviewById(int pageId)
    {
        var wjPage = this.wikiJs.Pages.FirstOrDefault(e => e.Id == pageId);
        if (wjPage is null)
        {
            return $"Page not found: {pageId}";
        }

        var sb = new StringBuilder();
        sb.AppendLine(wjPage.Render);
        sb.AppendLine();
        
        sb.AppendLine(string.Empty.PadRight(70, '-'));
        
        sb.AppendLine();
        var converter = ContentsConverter.Instance;
        sb.AppendLine(converter.GetNodePageContents(wjPage.Render));
        
        return sb.ToString();
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
    
    public async Task<string> UploadById(int wjPageId)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }

        var wjPage = this.wikiJs.Pages.FirstOrDefault(e => e.Id == wjPageId);
        if (wjPage is null)
        {
            return $"Page not found: {wjPageId}";
        }

        return await this.UploadFiles(wjPage);
    }

    public async Task<string> UploadFiles(int pageCount)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }

        int success = 0;
        int failed = 0;
        foreach (var wjPage in this.wikiJs.Pages.Take(pageCount))
        {
            var result = await this.UploadFiles(wjPage);
            Log.Info(result);
        }

        Log.Info($"success:{success} failed:{failed}");
        return "finished";
    }

    //// -----------------------------------------------------------------------------------------------

    public async Task<string> UploadFiles(WjPage wjPage)
    {
        if (this.CurrentSpace is null)
        {
            return "선택된 space가 없습니다.";
        }
        
        // 본문을 검색해 첨부해야 할 이미지가 있는지 확인한다.
        var converter = ContentsConverter.Instance;
        var files = converter.GetAttachmentFileList(wjPage.Render);
        if (files.Count == 0)
        {
            return $"pageId:{wjPage.Id} No image found.";
        }

        // 대응하는 cfPage를 찾는다.
        var cfPage = this.CurrentSpace.Pages.FirstOrDefault(e => e.Title == wjPage.UniqueTitle);
        if (cfPage is null)
        {
            return $"pageId:{wjPage.Id} cfPage not found: {wjPage.UniqueTitle}";
        }

        // cfPage의 현재 이미지 첨부 상황을 확인한다.
        await cfPage.CacheAttachmentState(this.client);

        // 업로드가 필요한 이미지를 선별한다.
        var uploadFiles = files.Where(e => cfPage.Attachments.All(a => a.Title != Path.GetFileName(e))).ToList();
        if (uploadFiles.Count == 0)
        {
            return $"pageId:{wjPage.Id} - 모든 파일이 이미 첨부됨. 파일 수:{files.Count}";
        }

        // 존재하지 않는 파일이 있다면 제외한다.
        var fullPaths = new List<string>();
        foreach (var file in uploadFiles)
        {
            if (!this.wikiJs.GetAssetPath(file, out var fullPath))
            {
                Log.Warn($"pageId:{wjPage.Id} asset file not exist: {file}");
                continue;
            }

            fullPaths.Add(fullPath);
        }

        Log.Debug($"allFiles:{files.Count} attached:{cfPage.Attachments.Count} uploadFiles:{uploadFiles.Count}");

        if (await cfPage.UploadFiles(this.client, fullPaths) == false)
        {
            return $"pageId:{wjPage.Id} Failed to upload files: {wjPage.UniqueTitle}";
        }

        Log.Debug($"pageId:{wjPage.Id} new file uploaded. files:{string.Join(Environment.NewLine, uploadFiles)}");

        // 새로 파일이 올라간 페이지는 자동 본문 새로고침
        return await this.ConvertById(wjPage.Id, force: true);
    }
}
