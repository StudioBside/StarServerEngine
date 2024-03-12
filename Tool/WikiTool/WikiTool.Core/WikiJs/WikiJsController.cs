namespace WikiTool.Core;

using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;

public sealed class WikiJsController
{
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
        return true;
    }
}
