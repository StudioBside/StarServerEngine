namespace Du.Presentation.Behaviors;

using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Microsoft.Xaml.Behaviors;

public sealed class TextBlockColorBehavior : Behavior<TextBlock>
{
    private DependencyPropertyDescriptor? textPropertyDescriptor;
    private bool onProcessing;

    //// --------------------------------------------------------------------

    protected override void OnAttached()
    {
        this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;

        this.textPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        if (this.textPropertyDescriptor != null)
        {
            this.textPropertyDescriptor.AddValueChanged(this.AssociatedObject, this.OnTextChanged);
        }
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.Loaded -= this.AssociatedObject_Loaded;

        if (this.textPropertyDescriptor != null)
        {
            this.textPropertyDescriptor.RemoveValueChanged(this.AssociatedObject, this.OnTextChanged);
        }
    }

    private static void ApplyColors(TextBlock textBlock, string text)
    {
        if (textBlock == null || string.IsNullOrEmpty(text))
        {
            return;
        }

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

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.onProcessing)
        {
            return;
        }

        this.onProcessing = true;
        ApplyColors(this.AssociatedObject, this.AssociatedObject.Text);
        this.onProcessing = false;
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        if (this.onProcessing)
        {
            return;
        }

        this.onProcessing = true;
        ApplyColors(this.AssociatedObject, this.AssociatedObject.Text);
        this.onProcessing = false;
    }
}
