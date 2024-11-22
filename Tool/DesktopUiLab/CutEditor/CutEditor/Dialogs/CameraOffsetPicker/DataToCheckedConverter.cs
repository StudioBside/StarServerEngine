namespace CutEditor.Dialogs.CameraOffsetPicker;

using System.Globalization;
using System.Windows;
using Du.Presentation.Converters;
using Du.Presentation.Extensions;
using static CutEditor.Model.Enums;

public sealed class DataToCheckedConverter : ConverterMarkupExtension<DataToCheckedConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case CameraOffsetCategory category:
                return category.ToString() == parameter.ToString();

            case int position:
                return int.TryParse(parameter.ToString(), out var paramInt) && paramInt == position;
        }

        return DependencyProperty.UnsetValue;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
