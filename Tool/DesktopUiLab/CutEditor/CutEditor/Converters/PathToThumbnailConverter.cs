namespace CutEditor.Converters;

using System.Globalization;
using System.Windows;
using Du.Presentation.Converters;
using Du.Presentation.Util;

public sealed class PathToThumbnailConverter : ConverterMarkupExtension<PathToThumbnailConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string strValue)
        {
            return DependencyProperty.UnsetValue;
        }

        return ImageHelper.GetThumbnail(strValue, thumbnailWidth: 200)
            ?? DependencyProperty.UnsetValue;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
