namespace CutEditor.Converters;

using System.Globalization;
using System.Windows;
using CutEditor.Services;
using Du.Presentation.Converters;
using NKM;

public sealed class CameraOffsetToImageConverter : ConverterMarkupExtension<CameraOffsetToImageConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Enum.TryParse<CameraOffset>(value.ToString(), out var easeValue) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        return CameraOffsetController.Instance.GetImageSource(easeValue);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
