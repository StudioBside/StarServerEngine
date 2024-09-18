namespace Du.Excel;

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Cs.Logging;
using Du.Core.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public sealed class CollectionEditor : ICollectionEditor
{
    public void Edit<T>(IList<T> collection)
    {
        // create temp file
        var tempFile = $"{Path.GetTempFileName()}.xlsx";
        Log.Debug($"tempFile: {tempFile}");

        // create excel file
        using (var stream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet excelSheet = workbook.CreateSheet("Sheet1");

            var columns = new List<string>();
            IRow row = excelSheet.CreateRow(0);
            int columnIndex = 0;

            foreach (var columnName in new[] { "Hello", "World" })
            {
                columns.Add(columnName);
                row.CreateCell(columnIndex).SetCellValue(columnName);
                columnIndex++;
            }

            int rowIndex = 1;
            foreach (var data in collection)
            {
                row = excelSheet.CreateRow(rowIndex);
                int cellIndex = 0;
                foreach (String col in columns)
                {
                    row.CreateCell(cellIndex).SetCellValue("values");
                    cellIndex++;
                }

                rowIndex++;
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
            return;
        }

        process.WaitForExit();
        Log.Debug("Edit completed");

        // read excel file
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

                for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                {
                    ICell cell = row.GetCell(cellIndex);
                    if (cell == null)
                    {
                        continue;
                    }

                    Log.Debug($"rowIndex: {rowIndex}, cellIndex: {cellIndex}, value: {cell.StringCellValue}");
                }
            }
        }
    }
}
