namespace Excel2Json.Functions;

using System.Data;
using Cs.Logging;
using Excel2Json.Binding;

internal sealed class UniquenessChecker
{
    private readonly UniquenessRecord record;
    private readonly HashSet<string> dataList = new();

    public UniquenessChecker(UniquenessRecord record)
    {
        this.record = record;
    }

    public bool CheckUniqueness(Source source)
    {
        bool result = true;
        if (this.CheckAllColumnExists(source.DataTable.Columns) == false)
        {
            return false;
        }

        foreach (DataRow dataRow in source.DataTable.Rows) // rows가 IEnumerable이 아니어서 SelectMany할 수 없다..
        {
            result &= this.AddData(dataRow);
        }

        return result;
    }

    //// ----------------------------------------------------------------------------------------------------------

    private bool AddData(DataRow dataRow)
    {
        List<string> buffer = new();
        foreach (var columnName in this.record.ColumnNames)
        {
            buffer.Add(dataRow[columnName].ToString() ?? string.Empty);
        }

        string data = string.Join(",", buffer);
        if (this.dataList.Contains(data))
        {
            ErrorContainer.Add($"중복된 키를 가진 데이터가 존재합니다. 규칙이름:{this.record.Name} value:{data}");
            return false;
        }

        this.dataList.Add(data);
        return true;
    }

    private bool CheckAllColumnExists(DataColumnCollection columnCollection)
    {
        foreach (var columnName in this.record.ColumnNames)
        {
            if (columnCollection.Contains(columnName) == false)
            {
                ErrorContainer.Add($"고유값 설정에 사용된 컬럼을 찾을 수 없습니다. 규칙이름:{this.record.Name} columnName:{columnName}");
                return false;
            }
        }

        return true;
    }
}
