namespace WikiTool.Core.ConfluenceTypes;

using System.Text;
using Cs.Logging;

public sealed class CfPage
{
    private readonly CfPageBulk bulk;
    private readonly List<CfPage> children = new();
    
    public CfPage(CfPageBulk bulk)
    {
        this.bulk = bulk;
    }
    
    public int Id => this.bulk.Id;
    public string Title => this.bulk.Title;
    public CfPage? Parent { get; private set; }
    
    public static void SetRelation(CfPage parent, CfPage child)
    {
        child.Parent = parent;
        parent.children.Add(child);
    }

    public override string ToString()
    {
        return this.DumpToString(indent: 0);
    }
    
    public void Join(IReadOnlyDictionary<int, CfPage> pages)
    {
        if (this.bulk.ParentId == 0)
        {
            // 부모가 없다면 조인할 일이 없다.
            return;
        }

        if (pages.TryGetValue(this.bulk.ParentId, out var parent) == false)
        {
            Log.Error($"Parent page not found: {this.bulk.ParentId}");
            return;
        }

        SetRelation(parent, this);
    }
    
    //// -------------------------------------------------------------------------------------
    
    private string DumpToString(int indent)
    {
        var space = string.Empty.PadRight(indent * 2, ' ');
        var sb = new StringBuilder();
        sb.AppendLine($"{space} - id:{this.Id} title:{this.Title} (parentId:{this.bulk.ParentId})");
        foreach (var child in this.children)
        {
            sb.AppendLine(child.DumpToString(indent + 1));
        }
        
        return sb.ToString();
    }
}
