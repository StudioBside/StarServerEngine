namespace Binder.Model.Detail;

using System.Text.Json;
using Cs.Core.Util;

public sealed class Source
{
    private string excelFile = string.Empty;
    private string sheetName = string.Empty;
    private string beginCell = "A1";

    public Source()
    {
    }

    public Source(JsonElement element)
    {
        this.excelFile = element.GetString("excelFile");
        this.sheetName = element.GetString("sheetName");
        this.beginCell = element.GetString("beginCell");
    }

    public string ExcelFile { get => this.excelFile; set => this.excelFile = value; }
    public string SheetName { get => this.sheetName; set => this.sheetName = value; }
    public string BeginCell { get => this.beginCell; set => this.beginCell = value; }
}
