namespace Du.Excel;

using System.Reflection;
using Cs.Core.Util;
using Du.Core.Interfaces;
using Du.Excel.Detail;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public sealed class ExcelFileWriter : IExcelFileWriter
{
    public bool Write<T>(string filePath, IEnumerable<T> collection)
    {
        FileSystem.GuaranteePath(filePath);

        var typeName = typeof(T).Name;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // create excel file
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

        IWorkbook workbook = new XSSFWorkbook();
        ISheet excelSheet = workbook.CreateSheet(typeName);

        IRow row = excelSheet.CreateRow(0);
        int columnIndex = 0;

        // 헤더(컬럼 이름)에는 별도의 스타일을 적용
        var style = workbook.CreateCellStyle();
        style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
        style.FillPattern = FillPattern.SolidForeground;

        // 헤더 생성
        foreach (var propertyInfo in properties)
        {
            var cell = row.CreateCell(columnIndex);
            cell.CellStyle = style;
            cell.SetCellValue(propertyInfo.Name);

            if (propertyInfo.PropertyType.IsEnum)
            {
                var delimiter = Environment.NewLine;
                var comment = $"value: {delimiter}{string.Join(delimiter, Enum.GetNames(propertyInfo.PropertyType))}";
                cell.AttachComment(comment);
            }

            columnIndex++;
        }

        // 데이터 생성
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

        return true;
    }
}
