namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;

public sealed class TruncateStringConverter : ConverterMarkupExtension<TruncateStringConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (int.TryParse(parameter?.ToString(), out var maxLength) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        if (value is not string stringValue)
        {
            return DependencyProperty.UnsetValue;
        }

        if (stringValue.Length <= maxLength)
        {
            return stringValue;
        }

        // return last part of the string
        return $"... {stringValue.Substring(stringValue.Length - maxLength)}";
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var parameterString = parameter as string;
        if (parameterString == null)
        {
            return DependencyProperty.UnsetValue;
        }

        return Enum.Parse(targetType, parameterString);
    }
}
