namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

public sealed class ColorToBrushConverter : ConverterMarkupExtension<ColorToBrushConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Color color)
        {
            return Binding.DoNothing;
        }

        return new SolidColorBrush(color);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
        {
            return Binding.DoNothing;
        }

        return brush.Color;
    }
}
