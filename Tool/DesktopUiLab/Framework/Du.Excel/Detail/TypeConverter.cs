namespace Du.Excel.Detail;

using System;
using System.Reflection;
using Cs.Logging;
using NPOI.SS.UserModel;

internal static class TypeConverter
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
        var type = property.PropertyType;
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
