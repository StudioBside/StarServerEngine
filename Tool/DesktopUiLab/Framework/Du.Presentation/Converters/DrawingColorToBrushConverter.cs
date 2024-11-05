namespace Du.Presentation.Converters;

using System;
using System.Globalization;
using System.Windows.Media;
using DrawingColor = System.Drawing.Color;

public sealed class DrawingColorToBrushConverter : ConverterMarkupExtension<DrawingColorToBrushConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DrawingColor drawingColor)
        {
            return Colors.Transparent; // 기본값
        }

        var color = Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
        return new SolidColorBrush(color);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
        {
            return DrawingColor.Empty; // 기본값
        }

        var mediaColor = brush.Color;
        return DrawingColor.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
    }
}
