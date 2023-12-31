namespace Excel2Json.ToEnum;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Antlr4.StringTemplate;

using Cs.Antlr;

using Excel2Json.Binding;

using Excel2Json.Functions;

using static Excel2Json.Enums;

internal sealed record ExtractEnum(
    string OutputFile,
    string NameSpace,
    FileOutputDirection FileOutDirection,
    Source Source)
    : IExtract
{
    private static readonly Column[] Columns;
    private static readonly UniquenessRecord[] Uniquenesses;

    static ExtractEnum()
    {
        Columns = new Column[]
        {
            new() { Name = ExcelEnumColumn.EnumTypeName.ToString(), DataType = DataType.String },
            new() { Name = ExcelEnumColumn.EnumBaseType.ToString(), DataType = DataType.String },
            new() { Name = ExcelEnumColumn.EnumLiteral.ToString(), DataType = DataType.String },
            new() { Name = ExcelEnumColumn.EnumValue.ToString(), DataType = DataType.Int32 },
            new() { Name = ExcelEnumColumn.DescriptionKor.ToString(), DataType = DataType.String, Nullable = true },
            new() { Name = ExcelEnumColumn.DescriptionEng.ToString(), DataType = DataType.String, Nullable = true },
        };

        Uniquenesses = new UniquenessRecord[]
        {
            new UniquenessRecord
            {
                Name = "mainKeyRule",
                ColumnNames = new[]
                {
                    ExcelEnumColumn.EnumTypeName.ToString(),
                    ExcelEnumColumn.EnumLiteral.ToString(),
                },
            },
        };
    }

    public string DebugName => $"[{this.OutputFile}]";

    public static Template CreateCsharpTemplate()
    {
        return StringTemplateFactory.Instance.Create("Template_enum.stg", "writeFile");
    }

    public bool Initialize()
    {
        if (this.Source.LoadExcel(this) == false)
        {
            return false;
        }

        foreach (var uniquenessRecord in Uniquenesses)
        {
            if (uniquenessRecord.Initialize(this) == false)
            {
                return false;
            }

            var checker = new UniquenessChecker(uniquenessRecord);
            if (checker.CheckUniqueness(this.Source) == false)
            {
                return false;
            }
        }

        return true;
    }

    bool IExtract.HasSourceFrom(IReadOnlySet<string> targetExcelFiles) => targetExcelFiles.Contains(this.Source.ExcelFile);
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
