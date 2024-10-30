﻿namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;

public sealed class EnumToCollapsedConverter : ConverterMarkupExtension<EnumToCollapsedConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var parameterString = parameter as string;
        if (parameterString == null)
        {
            return DependencyProperty.UnsetValue;
        }

        if (Enum.IsDefined(value.GetType(), value) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        object parameterValue = Enum.Parse(value.GetType(), parameterString);
        return parameterValue.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var parameterString = parameter as string;
        if (parameterString == null)
        {
            return DependencyProperty.UnsetValue;
        }

        return Enum.Parse(targetType, parameterString);
    }
}