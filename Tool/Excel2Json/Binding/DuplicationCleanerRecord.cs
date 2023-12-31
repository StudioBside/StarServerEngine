namespace Excel2Json.Binding;

using System;
using Cs.Logging;
using static Excel2Json.Enums;

internal sealed record DuplicationCleanerRecord
{
    public FileOutputDirection FileOutDirection { get; set; }
    public IReadOnlyList<string> ColumnNames { get; set; } = Array.Empty<string>();

    public bool Initialize(IExtract extract)
    {
        foreach (var columnName in this.ColumnNames)
        {
            if (extract.GetColumn(columnName, out var column) == false)
            {
                Log.Error($"{extract.DebugName} 중복 제거 설정상의 컬럼이 존재하지 않음. extract:{extract.DebugName} columnName:{columnName}");
                return false;
            }

            if (column.Nullable)
            {
                Log.Error($"{extract.DebugName} 중복 제거 설정에 포함된 컬럼은 nullable일 수 없습니다. extract:{extract.DebugName} columnName:{columnName}");
                return false;
            }
        }

        return true;
    }
}
