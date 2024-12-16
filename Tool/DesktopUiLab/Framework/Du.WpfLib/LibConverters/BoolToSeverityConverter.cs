namespace Du.WpfLib.LibConverters;

using System;
using System.Globalization;
using System.Windows.Data;
using Du.Presentation.Converters;
using Wpf.Ui.Controls;

public sealed class BoolToSeverityConverter : ConverterMarkupExtension<BoolToSeverityConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return Binding.DoNothing;
        }

        if (boolValue)
        {
            return InfoBarSeverity.Success;
        }

        if (parameter is string stringParameter &&
          stringParameter.Equals("Warning", StringComparison.OrdinalIgnoreCase))
        {
            return InfoBarSeverity.Warning;
        }

        return InfoBarSeverity.Error;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
