namespace Du.Presentation.Converters;

using System;
using System.Globalization;

public sealed class NullToBooleanConverter : ConverterMarkupExtension<NullToBooleanConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}