namespace CutEditor.Converters;

using System.Globalization;
using System.Windows;
using CutEditor.Services;
using Du.Presentation.Converters;
using static CutEditor.Model.Enums;

public sealed class EaseToImageConverter : ConverterMarkupExtension<EaseToImageConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Enum.TryParse<Ease>(value.ToString(), out var easeValue) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        if (EasingGraph.Instance.TryGetValue(easeValue, out var graph) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        return graph.ImageSource;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
