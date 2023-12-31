namespace Excel2Json.ToStringTable;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Antlr4.StringTemplate;

using Cs.Antlr;

using Excel2Json;
using Excel2Json.Binding;

using static Excel2Json.Enums;

internal sealed record ExtractString(
    string OutputFile,
    string NameSpace,
    FileOutputDirection FileOutDirection,
    List<Source> Sources)
    : IExtract
{
    private static readonly Column[] Columns;

    static ExtractString()
    {
        Columns = new Column[]
        {
            new() { Name = StringTableColumn.StringId.ToString(), DataType = DataType.String },
            new() { Name = StringTableColumn.StringValue.ToString(), DataType = DataType.String },
        };
    }

    public string DebugName => $"[{this.OutputFile}]";

    public static Template CreateAntlrTemplate()
    {
        return StringTemplateFactory.Instance.Create("Template_string.stg", "writeFile");
    }

    public bool Initialize()
    {
        foreach (var source in this.Sources)
        {
            if (source.LoadExcel(this) == false)
            {
                return false;
            }
        }

        //foreach (var uniquenessRecord in Uniquenesses)
        //{
        //    if (uniquenessRecord.Initialize(this) == false)
        //    {
        //        return false;
        //    }

        //    var checker = new UniquenessChecker(uniquenessRecord);
        //    if (checker.CheckUniqueness(this.Source) == false)
        //    {
        //        return false;
        //    }
        //}

        //if (this.BindRoot.Initialize(this) == false)
        //{
        //    return false;
        //}
        return true;
    }

    public string GetFinalTextOutput(FileOutputDirection fileOutDir)
    {
        var config = Config.Instance;

        return fileOutDir switch
        {
            FileOutputDirection.Server => config.Path.ServerTextOutput,
            FileOutputDirection.Client => config.Path.ClientTextOutput,
            FileOutputDirection.Tool => config.Path.ToolTextOutput,
            _ => throw new Exception($"invalid outputDirection:{fileOutDir}"),
        };
    }

    public string GetFinalBinOutput(FileOutputDirection fileOutDir)
    {
        var config = Config.Instance;
        return fileOutDir switch
        {
            FileOutputDirection.Server => config.Path.ServerBinOutput,
            FileOutputDirection.Client => config.Path.ClientBinOutput,
            FileOutputDirection.Tool => config.Path.ToolBinOutput,
            _ => throw new Exception($"invalid outputDirection:{fileOutDir}"),
        };
    }

    bool IExtract.HasSourceFrom(IReadOnlySet<string> targetExcelFiles)
    {
        // 인자로 받은 엑셀파일은 파일 이름만 들어있고, 멤버로 가진 sources는 상대경로를 갖고있다.
        foreach (var source in this.Sources)
        {
            if (targetExcelFiles.Any(source.ExcelFile.Contains))
            {
                return true;
            }
        }

        return false;
    }

    IEnumerable<Column> IExtract.GetAllColumns() => Columns;
    bool IExtract.HasColumn(string columnName) => Columns.Any(e => e.Name == columnName);
    bool IExtract.GetColumn(string name, [MaybeNullWhen(false)] out Column result)
    {
        result = Columns.FirstOrDefault(e => e.Name == name);
        return result != null;
    }

    void IExtract.AddSystemColumn(SystemColumn systemColumn)
    {
        throw new NotImplementedException();
    }
}
