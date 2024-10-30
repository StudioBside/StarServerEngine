namespace Du.Presentation.Converters;

using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

public sealed class ListToCountConverter : ConverterMarkupExtension<ListToCountConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IList list)
        {
            return Binding.DoNothing;
        }

        return list.Count;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
