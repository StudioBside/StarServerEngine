namespace CutEditor.Converters;

using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Du.Presentation.Converters;

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
