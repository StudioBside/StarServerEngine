namespace Du.Excel.Detail;

using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Cs.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

internal static class CellExt
{
    public static void Write(this ICell cell, object instance, PropertyInfo property)
    {
        var type = property.PropertyType;
        var objValue = property.GetValue(instance);
        if (objValue is null)
        {
            return;
        }

        if (type == typeof(string))
        {
            cell.SetCellValue(objValue as string);
        }
        else if (type == typeof(int))
        {
            cell.SetCellValue((int)objValue);
        }
        else if (type == typeof(double))
        {
            cell.SetCellValue((double)objValue);
        }
        else if (type == typeof(DateTime))
        {
            cell.SetCellValue((DateTime)objValue);
        }
        else if (type == typeof(bool))
        {
            cell.SetCellValue((bool)objValue);
        }
        else
        {
            cell.SetCellValue(objValue.ToString());
        }
    }

    public static bool Read(this ICell cell, object instance, PropertyInfo property)
    {
        return ReadCellValue(cell, instance, property, property.PropertyType);
    }

    public static void AttachComment(this ICell cell, string message)
    {
        var lineCount = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length;

        IDrawing drawing = cell.Sheet.CreateDrawingPatriarch();

        var address = cell.Address;
        var rectStart = new Point(address.Column + 1, address.Row + 1);
        var rectEnd = new Point(rectStart.X + 2, rectStart.Y + lineCount);
        IClientAnchor anchor = drawing.CreateAnchor(0, 0, 0, 0, rectStart.X, rectStart.Y, rectEnd.X, rectEnd.Y);

        IComment comment = drawing.CreateCellComment(anchor);
        comment.String = new XSSFRichTextString(message);
        comment.Author = "Du.Excel";

        // Assign the comment to the cell
        cell.CellComment = comment;
    }

    private static bool ReadCellValue(this ICell cell, object instance, PropertyInfo property, Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            if (cell.CellType == CellType.Blank)
            {
                property.SetValue(instance, null);
                return true;
            }

            return ReadCellValue(cell, instance, property, type.GenericTypeArguments[0]);
        }

        if (type == typeof(string))
        {
            property.SetValue(instance, cell.StringCellValue);
            return true;
        }

        if (type == typeof(int))
        {
            property.SetValue(instance, (int)cell.NumericCellValue);
            return true;
        }

        if (type == typeof(double))
        {
            property.SetValue(instance, cell.NumericCellValue);
            return true;
        }

        if (type == typeof(DateTime))
        {
            property.SetValue(instance, cell.DateCellValue);
            return true;
        }

        if (type == typeof(bool))
        {
            property.SetValue(instance, cell.BooleanCellValue);
            return true;
        }

        if (type.IsEnum)
        {
            if (Enum.TryParse(type, cell.StringCellValue, out var value) == false)
            {
                Log.Error($"value {cell.StringCellValue} is not valid enum literal. enum:{type.Name}");
                return false;
            }

            property.SetValue(instance, value);
            return true;
        }

        Log.Error($"Unsupported type {type.Name}");
        return false;
    }
}
