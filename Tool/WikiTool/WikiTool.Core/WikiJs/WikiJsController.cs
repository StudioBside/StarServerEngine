namespace WikiTool.Core;

using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;

public sealed class WikiJsController
{
    private readonly WjPath rootPath = new("root");
    
    public static WikiJsController Instance => Singleton<WikiJsController>.Instance;
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

        var pages = JsonUtil.Load<List<WjPage>>(pagesPath);
        if (pages is null)
        {
            Log.Error($"{this.DebugName} Failed to load pages: {pagesPath}");
            return false;
        }

        Log.Info($"{this.DebugName} Loaded {pages.Count} pages.");
        
        // build path
        foreach (var page in pages)
        {
            var path = page.Path;
            var pathParts = path.Split('/')[..^1]; // 마지막 경로는 곧 페이지의 이름
            var currentPath = this.rootPath;
            foreach (var part in pathParts)
            {
                if (currentPath.TryGetSubPath(part, out var subPath) == false)
                {
                    subPath = new WjPath(part);
                    currentPath.AddSubPath(subPath);
                }

                currentPath = subPath;
            }

            currentPath.AddPage(page);
        }

        // draw path to stdout
        DrawPath(this.rootPath, 0);
        
        return true;
    }

    //// --------------------------------------------------------------------------------
    
    private static void DrawPath(WjPath path, int depth)
    {
        var indent = new string(' ', depth * 2);
        Log.Info($"{indent}{path.Name}");
        foreach (var page in path.Pages)
        {
            Log.Info($"{indent}  - {page.Title} (path: {page.Path})");
        }

        foreach (var subPath in path.SubPaths)
        {
            DrawPath(subPath, depth + 1);
        }
    }
}
