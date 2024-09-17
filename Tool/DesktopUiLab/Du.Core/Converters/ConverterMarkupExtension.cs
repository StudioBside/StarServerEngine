namespace Du.Core.Converters;

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter
      where T : class, new()
{
    private static readonly Lazy<T> Converter = new(() => new T());

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Converter.Value;
    }

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
}
