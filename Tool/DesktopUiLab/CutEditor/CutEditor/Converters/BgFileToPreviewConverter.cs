namespace CutEditor.Converters;

using System.Globalization;
using System.IO;
using System.Windows;
using Du.Presentation.Converters;
using Du.Presentation.Util;

public sealed class BgFileToPreviewConverter : ConverterMarkupExtension<BgFileToPreviewConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string strValue)
        {
            return DependencyProperty.UnsetValue;
        }

        if (strValue.EndsWith(".png"))
        {
            var source = ImageHelper.CreateThumbnail(strValue, thumbnailWidth: 100);
            return new Wpf.Ui.Controls.Image
            {
                Source = source,
                CornerRadius = new CornerRadius(4),
            };
        }

        var ext = Path.GetExtension(strValue);
        if (string.IsNullOrEmpty(ext) != false)
        {
            return "enum 값";
        }

        return $"{ext} 파일";
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
