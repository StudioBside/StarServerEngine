﻿namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;

public sealed class NullToVisibilityConverter : ConverterMarkupExtension<NullToVisibilityConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string stringParameter &&
            stringParameter.Equals("Invert", StringComparison.OrdinalIgnoreCase))
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }
            
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}