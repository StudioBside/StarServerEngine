namespace Du.Presentation.Converters;

using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

public sealed class ListToStringConverter : ConverterMarkupExtension<ListToStringConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IList list)
        {
            return Binding.DoNothing;
        }

        if (list.Count == 0)
        {
            return Binding.DoNothing;
        }

        return string.Join(", ", list.Cast<object>());
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
