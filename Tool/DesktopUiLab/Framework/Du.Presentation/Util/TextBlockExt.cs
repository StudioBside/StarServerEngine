﻿namespace Du.Presentation.Util;

using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;

public static class TextBlockExt
{
    public static bool HasColorTag(string text)
    {
        return string.IsNullOrEmpty(text) == false && Regex.IsMatch(text, @"<color=#[0-9A-Fa-f]{6}>.*?</color>");
    }

    public static void ApplyColor(this TextBlock textBlock, string text)
    {
        if (textBlock == null || string.IsNullOrEmpty(text))
        {
            return;
        }

        // <b> 태그는 그냥 무시합니다
        text = text
            .Replace("<b>", string.Empty)
            .Replace("</b>", string.Empty);

        textBlock.Inlines.Clear();

        string pattern = @"<color=#(?<color>[0-9A-Fa-f]{6})>(?<text>.*?)<\/color>";
        var matches = Regex.Matches(text, pattern);

        int lastIndex = 0;
        foreach (Match match in matches)
        {
            // Add text before the match
            if (match.Index > lastIndex)
            {
                textBlock.Inlines.Add(new Run(text.Substring(lastIndex, match.Index - lastIndex)));
            }

            // Add colored text
            string colorCode = match.Groups["color"].Value;
            string coloredText = match.Groups["text"].Value;
            var run = new Run(coloredText)
            {
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{colorCode}")),
            };

            var shadowedTextBlock = new TextBlock(run)
            {
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 1,
                    BlurRadius = 0,
                },
            };
            textBlock.Inlines.Add(new InlineUIContainer(shadowedTextBlock));

            lastIndex = match.Index + match.Length;
        }

        // Add remaining text after the last match
        if (lastIndex < text.Length)
        {
            textBlock.Inlines.Add(new Run(text.Substring(lastIndex)));
        }
    }
}
