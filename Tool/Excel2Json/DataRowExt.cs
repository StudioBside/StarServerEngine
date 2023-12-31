namespace Excel2Json;

using System.Data;

using static Excel2Json.Enums;

internal static class DataRowExt
{
    public static string GetString(this DataRow self, string columnName)
    {
        return self[columnName].ToString() ?? string.Empty;
    }

    public static string GetString(this DataRow self, ExcelEnumColumn excelEnumColumn)
    {
        return self[excelEnumColumn.ToString()].ToString() ?? string.Empty;
    }

    public static string GetString(this DataRow self, SystemColumn systemColumn)
    {
        return self[systemColumn.ToString()].ToString() ?? string.Empty;
    }

    public static int GetInt32(this DataRow self, string columnName)
    {
        var value = self.GetString(columnName);
        int.TryParse(value, out var result);
        return result;
    }

    public static int GetInt32(this DataRow self, ExcelEnumColumn excelEnumColumn)
    {
        var value = self.GetString(excelEnumColumn);
        int.TryParse(value, out var result);
        return result;
    }

    public static bool GetBool(this DataRow self, string columnName)
    {
        var value = self.GetString(columnName);
        bool.TryParse(value, out var result);
        return result;
    }
}
