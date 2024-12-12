namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

public sealed class BoolToVisibilityConverter : ConverterMarkupExtension<BoolToVisibilityConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return Binding.DoNothing;
        }

        if (parameter is string stringParameter &&
          stringParameter.Equals("Invert", StringComparison.OrdinalIgnoreCase))
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
