namespace Du.Presentation.Converters;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;

public sealed class EnumDescriptionConverter : ConverterMarkupExtension<EnumDescriptionConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return DependencyProperty.UnsetValue;
        }

        if (Enum.IsDefined(value.GetType(), value) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        var field = value.GetType().GetField(value.ToString()!);
        if (field is null)
        {
            return DependencyProperty.UnsetValue;
        }

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute == null ? value : attribute.Description;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
