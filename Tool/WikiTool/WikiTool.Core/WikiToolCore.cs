namespace WikiTool.Core;

using Cs.Core.Util;

public sealed class WikiToolCore
{
    private readonly WikiToolConfig config;

    public WikiToolCore()
    {
        this.config = JsonUtil.Load<WikiToolConfig>("wikitool.config.json");
    }
}
