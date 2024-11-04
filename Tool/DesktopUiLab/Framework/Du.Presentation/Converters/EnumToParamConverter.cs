namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;

public sealed class EnumToParamConverter : ConverterMarkupExtension<EnumToParamConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Enum.IsDefined(value.GetType(), value) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        var enumValue = (int)value;
        var tokens = parameter?.ToString()?.Split('.');
        if (tokens is null || tokens.Length < enumValue)
        {
            return DependencyProperty.UnsetValue;
        }

        return tokens[enumValue];
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
