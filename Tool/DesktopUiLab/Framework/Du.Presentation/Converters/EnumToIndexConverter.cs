namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;

public sealed class EnumToIndexConverter : ConverterMarkupExtension<EnumToIndexConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Enum.IsDefined(value.GetType(), value) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        if (value is not Enum enumValue)
        {
            return DependencyProperty.UnsetValue;
        }

        return System.Convert.ToInt32(enumValue);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index && targetType.IsEnum)
        {
            return Enum.ToObject(targetType, index);
        }

        return DependencyProperty.UnsetValue;
    }
}
