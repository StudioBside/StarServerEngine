namespace CutEditor.Converters;

using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Du.Presentation.Converters;

/// <summary>
/// 이미지 경로를 이미지로 변환하는 다중 값 변환기입니다.
/// <MultiBinding Converter="{lcvt:PathToImageConverter}">
///     <Binding Path="Cut.Unit.SmallImageFullPath" />
///     <Binding Source="유닛 선택"/>
///     <Binding Source="CornerRadius:8;BorderBrush:Black;BorderThickness:1"/>
/// </MultiBinding>
/// </summary>
[Obsolete("사용하지는 않습니다. MultiValueConverterMarkupExtension 상속 구현의 예시를 위해서만 남겨둡니다.")]
public class PathToImageConverter : MultiValueConverterMarkupExtension<PathToImageConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var imagePath = values[0] as string;
        var defaultText = values[1] as string;
        var options = values[2] as string ?? string.Empty;

        if (string.IsNullOrEmpty(imagePath))
        {
            return defaultText ?? string.Empty;
        }

        if (File.Exists(imagePath) == false)
        {
            return "NoFile";
        }

        var image = new Wpf.Ui.Controls.Image
        {
            Source = new BitmapImage(new Uri(imagePath)),
        };

        var tokens = options.Split(';');
        foreach (var token in tokens)
        {
            var parts = token.Split(':');
            if (parts.Length != 2)
            {
                continue;
            }

            var key = parts[0];
            var value = parts[1];

            switch (key)
            {
                case "CornerRadius":
                    image.CornerRadius = new CornerRadius(int.Parse(value));
                    break;

                case "BorderBrush":
                    var color = (Color)ColorConverter.ConvertFromString(value);
                    image.BorderBrush = new SolidColorBrush(color);
                    break;

                case "BorderThickness":
                    image.BorderThickness = new Thickness(int.Parse(value));
                    break;
            }
        }

        return image;
    }

    public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
