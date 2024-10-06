namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

public abstract class MultiValueConverterMarkupExtension<T> : MarkupExtension, IMultiValueConverter
      where T : class, new()
{
    private static readonly Lazy<T> Converter = new(() => new T());

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Converter.Value;
    }

    public abstract object Convert(object[] value, Type targetType, object parameter, CultureInfo culture);
    public abstract object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture);
}
