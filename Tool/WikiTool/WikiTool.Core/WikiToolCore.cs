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
    
    public string UploadImage(int wjPageId)
    {
        var wjPage = this.wikiJs.Pages.FirstOrDefault(e => e.Id == wjPageId);
        if (wjPage is null)
        {
            return $"Page not found: {wjPageId}";
        }

        return string.Empty;

        /*
        var converter = ContentsConverter.Instance;
        var imagePaths = converter.GetImagePaths(wjPage.Render);
        if (imagePaths.Count == 0)
        {
            return "No image found.";
        }

        var sb = new StringBuilder();
        foreach (var imagePath in imagePaths)
        {
            var image = new FileInfo(imagePath);
            if (image.Exists == false)
            {
                sb.AppendLine($"Image not found: {imagePath}");
                continue;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "wiki/api/v2/contentbody/convert/image");
            request.Content = new ByteArrayContent(File.ReadAllBytes(imagePath));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            var response = this.client.Send(request);
            if (response.IsSuccessStatusCode == false)
            {
                sb.AppendLine($"Failed to upload image: {imagePath}");
                continue;
            }

            var contentId = response.GetContentAs(obj => obj["id"]!.ToObject<int>());
            if (contentId is null)
            {
                sb.AppendLine($"Failed to get contentId: {imagePath}");
                continue;
            }

            var content = new JObject
            {
                ["id"] = contentId,
                ["type"] = "image",
                ["title"] = image.Name,
                ["mediaType"] = "image/png",
                ["metadata"] = new JObject
                {
                    ["comment"] = "uploaded by wikitool",
                },
            };
            var contentJson = JsonConvert.SerializeObject(content);
            var contentRequest = new HttpRequestMessage(HttpMethod.Post, $"wiki/api/v2/content/{wjPageId}/child/attachment");
            contentRequest.Content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            contentRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var contentResponse = this.client.Send(contentRequest);
            if (contentResponse.IsSuccessStatusCode == false)
            {
                sb.AppendLine($"Failed to upload image: {imagePath}");
                continue;
            }

            sb.AppendLine($"Uploaded image: {imagePath}");
        }

        return sb.ToString();
        */
    }
}
