namespace Du.Excel;

using System.Collections.Generic;
using System.Reflection;
using Cs.Logging;
using Du.Core.Interfaces;
using Du.Excel.Detail;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public sealed class ExcelFileReader : IExcelFileReader
{
    bool IExcelFileReader.Read<T>(string filePath, IList<T> collection)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyMap = new PropertyInfo[properties.Length]; // index가 cellIndex와 일치하도록 정렬.
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IWorkbook workbook = new XSSFWorkbook(stream);
        ISheet excelSheet = workbook.GetSheetAt(0);

        // row 0 에서 헤더를 먼저 읽어 propertyMap을 구성한다.
        IRow headerRow = excelSheet.GetRow(0);
        if (headerRow == null)
        {
            Log.Error("[CollectionEditor] Failed to read header row.");
            return false;
        }

        for (int cellIndex = 0; cellIndex < headerRow.LastCellNum; cellIndex++)
        {
            ICell cell = headerRow.GetCell(cellIndex);
            if (cell == null)
            {
                Log.Error($"[CollectionEditor] Failed to read header cell. cellIndex:{cellIndex}");
                return false;
            }

            var header = cell.ToString();
            var propertyInfo = properties.FirstOrDefault(x => x.Name == header);
            if (propertyInfo == null)
            {
                Log.Error($"[CollectionEditor] Failed to find property. header:{header}");
                return false;
            }

            propertyMap[cellIndex] = propertyInfo;
        }

        // row 1부터 데이터를 읽어 collection에 추가한다.
        for (int rowIndex = 1; rowIndex <= excelSheet.LastRowNum; rowIndex++)
        {
            IRow row = excelSheet.GetRow(rowIndex);
            if (row == null)
            {
                continue;
            }

            var instance = new T();
            collection.Add(instance);
            for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
            {
                var propertyInfo = propertyMap[cellIndex];
                ICell cell = row.GetCell(cellIndex);
                if (cell == null)
                {
                    continue;
                }

                if (cell.Read(instance, propertyInfo) == false)
                {
                    Log.Error($"[CollectionEditor] Failed to read cell. rowIndex:{rowIndex} cellIndex:{cellIndex} value:{cell.ToString()}");
                    return false;
                }
            }
        }

        return true;
    }
}
