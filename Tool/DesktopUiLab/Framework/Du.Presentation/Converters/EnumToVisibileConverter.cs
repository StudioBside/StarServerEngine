namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;

public sealed class EnumToVisibileConverter : ConverterMarkupExtension<EnumToVisibileConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var parameterString = parameter as string;
        if (parameterString == null)
        {
            return DependencyProperty.UnsetValue;
        }

        if (Enum.IsDefined(value.GetType(), value) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        foreach (var token in parameterString.Split('|'))
        {
            if (Enum.TryParse(value.GetType(), token, out var parameterValue) == false)
            {
                return DependencyProperty.UnsetValue;
            }

            if (parameterValue.Equals(value))
            {
                return Visibility.Visible;
            }
        }

        return Visibility.Collapsed;
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
