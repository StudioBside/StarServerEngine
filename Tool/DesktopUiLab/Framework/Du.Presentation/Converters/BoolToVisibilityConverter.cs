namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

public sealed class BoolToVisibilityConverter : IValueConverter
{
    public Visibility TrueValue { get; set; } = Visibility.Visible;
    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return Binding.DoNothing;
        }

        return boolValue ? this.TrueValue : this.FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
