namespace Du.Excel;

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Cs.Core.Util;
using Cs.Logging;
using Du.Core.Interfaces;
using Du.Excel.Detail;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public sealed class CollectionEditor : ICollectionEditor
{
    public async Task<bool> Edit<T>(IList<T> collection) where T : new()
    {
        // create temp file
        var tempFile = $"{Path.GetTempFileName()}.xlsx";
        Log.Debug($"tempFile: {tempFile}");

        var writer = new ExcelFileWriter();
        if (writer.Write(tempFile, collection) == false)
        {
            return false;
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

        await process.WaitForExitAsync();
        Log.Debug("Edit completed");

        // read excel file
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var newCollection = new List<T>();
        using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read))
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
