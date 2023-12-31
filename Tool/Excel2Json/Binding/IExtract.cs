namespace Excel2Json.Binding;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using static Excel2Json.Enums;

internal interface IExtract
{
    string OutputFile { get; }
    FileOutputDirection FileOutDirection { get; }
    public string DebugName { get; }

    bool HasColumn(string columnName);
    bool GetColumn(string name, [MaybeNullWhen(false)] out Column result);
    bool HasSourceFrom(IReadOnlySet<string> targetExcelFiles);
    IEnumerable<Column> GetAllColumns();
    void AddSystemColumn(SystemColumn systemColumn);
}
