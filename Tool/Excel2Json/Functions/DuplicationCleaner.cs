namespace Excel2Json.Functions;

using System.Data;
using Excel2Json.Binding;
using Excel2Json.ToNormalTemplet;
using static Excel2Json.Enums;

// 서버로만 뽑히는 데이터가 많은 경우, 클라이언트 데이터는 의미 없는 중복 데이터가 많이 생길 수 있다. 
// 이 때 client direction으로만 중복 제거를 걸어 첫 번째 데이터만 뽑히도록 만들어주는 역할을 한다.
internal sealed class DuplicationCleaner
{
    private readonly DuplicationCleanerRecord record;
    private readonly HashSet<string> dataList = new();

    public DuplicationCleaner(DuplicationCleanerRecord record)
    {
        this.record = record;
    }

    public static DuplicationCleaner? TryCreate(FileOutputDirection fileOutDir, Extract extract)
    {
        if (extract.DuplicationCleaner is null)
        {
            return null;
        }

        if (extract.DuplicationCleaner.FileOutDirection != fileOutDir)
        {
            return null;
        }

        return new(extract.DuplicationCleaner);
    }

    public bool CheckIfUniqueData(DataRow dataRow)
    {
        List<string> buffer = new();
        foreach (var columnName in this.record.ColumnNames)
        {
            buffer.Add(dataRow[columnName].ToString() ?? string.Empty);
        }

        string data = string.Join(",", buffer);
        if (this.dataList.Contains(data))
        {
            return false;
        }

        this.dataList.Add(data);
        return true;
    }
}
