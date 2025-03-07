namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

public sealed class IntToVisibilityConverter : ConverterMarkupExtension<IntToVisibilityConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int intValue)
        {
            return Binding.DoNothing;
        }

        var positiveType = Visibility.Visible;
        var negativeType = Visibility.Collapsed;

        if (parameter is string stringParameter)
        {
            if (stringParameter.Equals("Hidden", StringComparison.OrdinalIgnoreCase))
            {
                negativeType = Visibility.Hidden;
            }

            if (stringParameter.Equals("Invert", StringComparison.OrdinalIgnoreCase))
            {
                (positiveType, negativeType) = (negativeType, positiveType);
            }
        }

        return intValue > 0 ? positiveType : negativeType;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
