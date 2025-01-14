namespace Du.Excel;

using System.IO;
using System.Reflection;
using Cs.Core.Util;
using Du.Core.Interfaces;
using Du.Excel.Detail;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Log = Cs.Logging.Log;

public sealed class ExcelFileWriter : IExcelFileWriter
{
    private readonly Dictionary<string, SheetContext> openedSheets = new();
    private FileStream? fileStream;
    private IWorkbook? workbook;

    public bool Write<T>(string filePath, IEnumerable<T> collection)
    {
        if (this.CreateSheet<T>(filePath) == false)
        {
            return false;
        }

        if (this.AppendToSheet(collection) == false)
        {
            return false;
        }

        return this.CloseSheet<T>();
    }

    public bool CreateSheet<T>(string filePath)
    {
        var typeName = typeof(T).Name;
        if (this.openedSheets.ContainsKey(typeName))
        {
            Log.Warn($"Sheet '{typeName}' already opened.");
            return false;
        }

        FileSystem.GuaranteePath(filePath);

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        // setter가 없는 읽기 전용 프로퍼티는 제외.
        properties = properties.Where(x => x.CanWrite).ToArray();

        // create excel file
        this.fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

        this.workbook = new XSSFWorkbook();
        ISheet excelSheet = this.workbook.CreateSheet(typeName);

        IRow row = excelSheet.CreateRow(0);
        int columnIndex = 0;

        // 헤더(컬럼 이름)에는 별도의 스타일을 적용
        var style = this.workbook.CreateCellStyle();
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

        this.openedSheets.Add(typeName, new SheetContext(excelSheet, properties));
        return true;
    }

    public bool AppendToSheet<T>(IEnumerable<T> collection)
    {
        var typeName = typeof(T).Name;
        if (this.openedSheets.TryGetValue(typeName, out var context) == false)
        {
            Log.Warn($"Sheet '{typeName}' not opened.");
            return false;
        }

        foreach (var data in collection)
        {
            if (data is null)
            {
                continue;
            }

            var row = context.CreateRow();
            int cellIndex = 0;
            foreach (var propertyInfo in context.Properties)
            {
                row.CreateCell(cellIndex).Write(data, propertyInfo);
                cellIndex++;
            }
        }

        return true;
    }

    public bool CloseSheet<T>()
    {
        if (this.workbook is null)
        {
            Log.Warn("Workbook not created.");
            return false;
        }

        var typeName = typeof(T).Name;
        if (this.openedSheets.TryGetValue(typeName, out var context) == false)
        {
            Log.Warn($"Sheet '{typeName}' not opened.");
            return false;
        }

        for (int columnIndex = 0; columnIndex < context.Properties.Count; columnIndex++)
        {
            context.Sheet.AutoSizeColumn(columnIndex);
        }

        this.workbook.Write(this.fileStream);
        return true;
    }

    public void Dispose()
    {
        if (this.workbook is not null)
        {
            this.workbook.Close();
            this.workbook.Dispose();
            this.workbook = null;
        }

        if (this.fileStream is not null)
        {
            this.fileStream.Close();
            this.fileStream.Dispose();
            this.fileStream = null;
        }
    }

    //// ---------------------------------------------------------------------------------------------

    private sealed class SheetContext(ISheet sheet, IReadOnlyList<PropertyInfo> properties)
    {
        private int nextRowIndex = 1;

        public ISheet Sheet { get; } = sheet;
        public IReadOnlyList<PropertyInfo> Properties { get; } = properties;

        public IRow CreateRow() => this.Sheet.CreateRow(this.nextRowIndex++);
    }
}
