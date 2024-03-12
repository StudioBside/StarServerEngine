namespace WikiTool.Core.ConfluenceTypes;

public sealed class CfPage
{
    private readonly CfPageBulk bulk;
    
    public CfPage(CfPageBulk bulk)
    {
        this.bulk = bulk;
    }
    
    public int Id => this.bulk.Id;
}
