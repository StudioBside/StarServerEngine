namespace WikiTool.Core;

using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;

public sealed class WikiJsController
{
    private readonly List<WjPage> pages = new();
    
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
        return true;
    }

    public WjPage? GetByPath(string path)
    {
        // remove leading '/'
        if (path.StartsWith('/'))
        {
            path = path[1..];
        }
        
        return this.pages.FirstOrDefault(p => p.Path == path);
    }

    //// --------------------------------------------------------------------------------
}
