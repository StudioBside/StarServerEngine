namespace Du.Excel;

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Cs.Logging;
using Du.Core.Interfaces;
using Du.Excel.Detail;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public sealed class CollectionEditor : ICollectionEditor
{
    public bool Edit<T>(IList<T> collection) where T : new()
    {
        // create temp file
        var tempFile = $"{Path.GetTempFileName()}.xlsx";
        Log.Debug($"tempFile: {tempFile}");

        var typeName = typeof(T).Name;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // create excel file
        using (var stream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet excelSheet = workbook.CreateSheet(typeName);

            IRow row = excelSheet.CreateRow(0);
            int columnIndex = 0;

            var style = workbook.CreateCellStyle();
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;

            foreach (var propertyInfo in properties)
            {
                var cell = row.CreateCell(columnIndex);
                cell.CellStyle = style;
                cell.SetCellValue(propertyInfo.Name);
                columnIndex++;
            }

            int rowIndex = 1;
            foreach (var data in collection)
            {
                if (data is null)
                {
                    continue;
                }

                row = excelSheet.CreateRow(rowIndex);
                int cellIndex = 0;
                foreach (var propertyInfo in properties)
                {
                    row.CreateCell(cellIndex).Write(data, propertyInfo);
                    cellIndex++;
                }

                rowIndex++;
            }

            for (columnIndex = 0; columnIndex < properties.Length; columnIndex++)
            {
                excelSheet.AutoSizeColumn(columnIndex);
            }

            workbook.Write(stream);
        }

        // open excel as child process
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "excel.exe",
            Arguments = tempFile,
            UseShellExecute = true,
        };
        var process = Process.Start(processStartInfo);
        if (process is null)
        {
            Log.Error($"Failed to start excel.exe with {tempFile}");
            return false;
        }

        process.WaitForExit();
        Log.Debug("Edit completed");

        // read excel file
        var newCollection = new List<T>();
        using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet excelSheet = workbook.GetSheetAt(0);

            for (int rowIndex = 1; rowIndex <= excelSheet.LastRowNum; rowIndex++)
            {
                IRow row = excelSheet.GetRow(rowIndex);
                if (row == null)
                {
                    continue;
                }

                var instance = new T();
                newCollection.Add(instance);
                for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                {
                    var propertyInfo = properties[cellIndex];
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
        }

        // update collection
        collection.Clear();
        foreach (var item in newCollection)
        {
            collection.Add(item);
        }

        return true;
    }
}
