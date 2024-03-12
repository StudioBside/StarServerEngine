namespace WikiTool.Core;

using System.Diagnostics.CodeAnalysis;

public sealed class WjPath
{
    private readonly string name;
    private readonly List<WjPage> pages = new();
    private readonly Dictionary<string, WjPath> subPaths = new();

    public WjPath(string name)
    {
        this.name = name;
    }
    
    public string Name => this.name;
    public IReadOnlyList<WjPage> Pages => this.pages;
    public IEnumerable<WjPath> SubPaths => this.subPaths.Values;
    
    public bool TryGetSubPath(string name, [MaybeNullWhen(false)] out WjPath path)
    {
        return this.subPaths.TryGetValue(name, out path);
    }

    public void AddSubPath(WjPath path)
    {
        this.subPaths.Add(path.name, path);
    }
    
    public void AddPage(WjPage page)
    {
        this.pages.Add(page);
    }
}
