namespace Excel2Json.Binding;

using System;
using Cs.Logging;

internal sealed record UniquenessRecord
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<string> ColumnNames { get; set; } = Array.Empty<string>();

    public bool Initialize(IExtract extract)
    {
        foreach (var columnName in this.ColumnNames)
        {
            if (extract.GetColumn(columnName, out var column) == false)
            {
                Log.Error($"{extract.DebugName} 유니크 설정의 컬럼이 존재하지 않음. name:{this.Name} columnName:{columnName}");
                return false;
            }

            if (column.Nullable)
            {
                Log.Error($"{extract.DebugName} 유니크 설정에 포함된 컬럼은 nullable일 수 없습니다. name:{this.Name} columnName:{columnName}");
                return false;
            }
        }

        return true;
    }
}
