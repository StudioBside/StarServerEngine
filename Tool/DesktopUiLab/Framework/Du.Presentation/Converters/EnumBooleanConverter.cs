﻿namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Enum의 값에 따라 RadioButton의 값을 표기하기 위한 용도로 사용.
/// </summary>
public sealed class EnumBooleanConverter : ConverterMarkupExtension<EnumBooleanConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return DependencyProperty.UnsetValue;
        }

        var parameterString = parameter as string;
        if (parameterString == null)
        {
            return DependencyProperty.UnsetValue;
        }

        if (Enum.IsDefined(value.GetType(), parameterString) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        object parameterValue = Enum.Parse(value.GetType(), parameterString);
        return parameterValue.Equals(value);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var parameterString = parameter as string;
        if (parameterString == null)
        {
            return DependencyProperty.UnsetValue;
        }

        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType is not null)
        {
            targetType = underlyingType;
        }

        return Enum.Parse(targetType, parameterString);
    }
}
