namespace WikiTool.Core;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using WikiTool.Core.Transform;
using WikiTool.Core.WikiJs;

public sealed class WikiJsController
{
    private readonly List<WjPage> pages = new();
    private readonly AssetController assetController = new();

    public static WikiJsController Instance => Singleton<WikiJsController>.Instance;
    public IReadOnlyList<WjPage> Pages => this.pages;
    private string DebugName => $"[WikiJs]";
    
    public bool Initialize(string backupPath)
    {
        if (Directory.Exists(backupPath) == false)
        {
            Log.Error($"{this.DebugName} Backup path not found: {backupPath}");
            return false;
        }

        // load pages.json
        var pagesPath = Path.Combine(backupPath, "pages.json");
        if (File.Exists(pagesPath) == false)
        {
            Log.Error($"{this.DebugName} Pages file not found: {pagesPath}");
            return false;
        }

        this.pages.AddRange(JsonUtil.Load<List<WjPage>>(pagesPath));
        if (this.pages.Any() == false)
        {
            Log.Error($"{this.DebugName} Failed to load pages: {pagesPath}");
            return false;
        }

        Log.Info($"{this.DebugName} Loaded {this.pages.Count} pages.");

        var builder = new UniquePathBuilder();
        foreach (var page in this.pages)
        {
            page.PathTokens = builder.Calculate(page.Path);
        }

        // load assets
        this.assetController.Initialize(Path.Combine(backupPath, "assets"));

        return true;
    }

    public WjPage? GetByPath(string path)
    {
        // remove leading '/'
        if (path.StartsWith('/'))
        {
            path = path[1..];
        }
        
        var result = this.pages.FirstOrDefault(p => p.Path == path);
        if (result is not null)
        {
            return result;
        }

        // unescape path
        var unescapedPath = Uri.UnescapeDataString(path);
        result = this.pages.FirstOrDefault(p => p.Path == unescapedPath);
        if (result is not null)
        {
            return result;
        }

        return null;
    }

    public string ValidateAsset()
    {
        var sb = new StringBuilder();

        // 모든 페이지를 돌면서 첨부파일 여부를 확인하고, asset에 파일 존재 여부를 검증한다.
        var converter = ContentsConverter.Instance;
        bool successful = true;
        foreach (var wjPage in this.pages)
        {
            var files = converter.GetAttachmentFileList(wjPage.Render);
            if (files.Count == 0)
            {
                continue;
            }

            foreach (var file in files)
            {
                if (this.GetAssetPath(file, out _) == false)
                {
                    sb.AppendLine($"Asset not found: pageId:{wjPage.Id} missingFile:{file}");
                    successful = false;
                }
            }
        }

        if (successful)
        {
            sb.AppendLine("All assets are valid.");
        }

        return sb.ToString();
    }

    public bool GetAssetPath(string path, [MaybeNullWhen(false)] out string fullPath)
    {
        return this.assetController.Contains(path, out fullPath);
    }

    //// --------------------------------------------------------------------------------
}
