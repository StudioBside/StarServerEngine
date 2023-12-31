namespace Excel2Json.Binding;

using System;
using System.Data;
using Cs.Logging;
using Excel2Json;
using ExcelDataReader;
using static Excel2Json.Enums;

internal sealed class Source
{
    public string ExcelFile { get; set; } = string.Empty;
    public string SheetName { get; set; } = string.Empty;
    public string BeginCell { get; set; } = "A1";
    public DataTable DataTable { get; private set; } = null!;
    public bool HasStableColumn { get; private set; }
    public long FileSize { get; private set; }

    public string DebugName => $"[{this.ExcelFile}/{this.SheetName}]";

    public bool LoadExcel(IExtract extract)
    {
        var config = Config.Instance;
        var fileFullPath = Path.Combine(config.Path.ExcelInput, this.ExcelFile);
        if (File.Exists(fileFullPath) == false)
        {
            Log.Error($"{this.DebugName} 엑셀 파일을 찾을 수 없습니다.");
            return false;
        }

        if (this.BeginCell.Length != 2
            || GetAlphabetIndex(this.BeginCell[0], out int startColumnIndex) == false
            || char.IsDigit(this.BeginCell[1]) == false)
        {
            Log.Error($"{this.DebugName} 데이터 시작점 값이 올바르지 않습니다. BeginCell:{this.BeginCell}");
            return false;
        }

        int startRowIndex = int.Parse(this.BeginCell[1..]) - 1; // move to zero-base

        var dataSetConfig = new ExcelDataSetConfiguration
        {
            FilterSheet = (reader, index) => reader.Name == this.SheetName,
            ConfigureDataTable = reader => new()
            {
                FilterRow = reader => reader.Depth >= startRowIndex,
                FilterColumn = (reader, index) => index >= startColumnIndex,
            },
            UseColumnDataType = false,
        };

        DataSet? dataset;
        using (var stream = File.Open(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using var reader = ExcelReaderFactory.CreateReader(stream);
            dataset = reader.AsDataSet(dataSetConfig);
        }

        this.FileSize = new FileInfo(fileFullPath).Length;

        if (dataset.Tables.Contains(this.SheetName) == false)
        {
            Log.Error($"{this.DebugName} 시트 이름이 올바르지 않습니다.");
            return false;
        }

        var dataTable = dataset.Tables[this.SheetName];
        if (dataTable == null)
        {
            Log.Error($"{this.DebugName} 엑셀 시트 로딩 실패.");
            return false;
        }

        this.DataTable = dataTable;

        // 데이터의 첫 번째 row를 이름으로 처리한다.
        HashSet<string> requiredColumns = new();
        List<string> needlessColumns = new();
        DataRow firstRow = this.DataTable.Rows[0];
        foreach (DataColumn column in this.DataTable.Columns)
        {
            string name = firstRow.GetString(column.ColumnName);
            if (string.IsNullOrEmpty(name))
            {
                Log.Error($"{this.DebugName} 컬럼 이름이 존재하지 않아 제거합니다. column.Ordinal:{column.Ordinal}");

                column.ColumnName = $"_garbage_column_{column.Ordinal}";
                needlessColumns.Add(column.ColumnName);
                continue;
            }

            column.ColumnName = name;

            if (extract.HasColumn(name))
            {
                requiredColumns.Add(name);
            }
            else if (name.Equals(nameof(SystemColumn.IsStable)))
            {
                this.HasStableColumn = true;
                requiredColumns.Add(name);
            }
            else if (name.Equals(nameof(SystemColumn.ContentsTag)))
            {
                requiredColumns.Add(name);
                extract.AddSystemColumn(SystemColumn.ContentsTag);
            }
            else
            {
                needlessColumns.Add(name);
            }
        }

        // 이름 설정이 끝나면 첫 줄 데이터는 버림.
        this.DataTable.Rows.Remove(firstRow);

        // 불필요한 컬럼들도 삭제.
        if (needlessColumns.Count > 0)
        {
            foreach (string needless_colum in needlessColumns)
            {
                this.DataTable.Columns.Remove(needless_colum);
            }
        }

        // 필요한 컬럼이 모두 존재하는지 확인
        bool result = true;
        foreach (var column in extract.GetAllColumns())
        {
            if (requiredColumns.Contains(column.Name) == false)
            {
                Log.Error($"{this.DebugName} 컬럼 {column.Name}을 찾을 수 없습니다.");
                result = false;
            }
        }

        Log.Debug($"{this.DebugName} loading ... ok");
        return result;
    }

    private static bool GetAlphabetIndex(char value, out int result)
    {
        if (value >= 'A' && value <= 'Z')
        {
            result = value - 'A';
            return true;
        }

        if (value >= 'a' && value <= 'z')
        {
            result = value - 'a';
            return true;
        }

        result = -1;
        return false;
    }
}
