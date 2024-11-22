namespace CutEditor.Dialogs.CameraOffsetPicker;

using System.Globalization;
using System.Windows;
using Du.Presentation.Converters;
using Du.Presentation.Extensions;
using static CutEditor.Model.Enums;

public sealed class PositionEnableConverter : ConverterMarkupExtension<PositionEnableConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CameraOffsetCategory category ||
            parameter is not string str)
        {
            return DependencyProperty.UnsetValue;
        }

        if (int.TryParse(str, out var paramInt) == false)
        {
            return DependencyProperty.UnsetValue;
        }

        switch (category)
        {
            case CameraOffsetCategory.None:
            case CameraOffsetCategory.Default:
            case CameraOffsetCategory.All:
                return false;

            case CameraOffsetCategory.Twin:
                return paramInt <= 7;

            case CameraOffsetCategory.Triple:
                return paramInt <= 5;
            default:
                return true;
        }
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
