namespace Du.Presentation.Behaviors;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Du.Presentation.Util;
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

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.onProcessing)
        {
            return;
        }

        this.onProcessing = true;
        this.AssociatedObject.ApplyColor(this.AssociatedObject.Text);
        this.onProcessing = false;
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        if (this.onProcessing)
        {
            return;
        }

        this.onProcessing = true;
        this.AssociatedObject.ApplyColor(this.AssociatedObject.Text);
        this.onProcessing = false;
    }
}
