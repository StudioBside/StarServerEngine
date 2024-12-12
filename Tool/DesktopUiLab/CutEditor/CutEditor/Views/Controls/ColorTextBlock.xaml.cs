namespace CutEditor.Views.Controls;

using System.Windows;
using System.Windows.Controls;
using Du.Presentation.Util;

public partial class ColorTextBlock : UserControl
{
    private static readonly DependencyProperty TextProperty;

    static ColorTextBlock()
    {
        TextProperty = DependencyProperty.Register(
            name: nameof(Text),
            propertyType: typeof(string),
            ownerType: typeof(ColorTextBlock),
            typeMetadata: new PropertyMetadata(
                defaultValue: null,
                propertyChangedCallback: TextChanged));
    }

    public ColorTextBlock()
    {
        this.InitializeComponent();
    }

    public string Text
    {
        get => (string)this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }

    //// --------------------------------------------------------------------

    private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorTextBlock)d;
        if (control == null)
        {
            return;
        }

        var newText = e.NewValue as string ?? string.Empty;
        control.SourceBlock.Text = newText;
        control.ColorBlock.ApplyColor(newText);

        var hasColorTag = TextBlockExt.HasColorTag(newText);
        control.ShowColor.IsChecked = hasColorTag;
        control.ShowColor.Visibility = hasColorTag ? Visibility.Visible : Visibility.Hidden;
        control.UpdateViewState();
    }

    private void ShowColor_Checked(object sender, RoutedEventArgs e)
    {
        this.UpdateViewState();
    }

    private void UpdateViewState()
    {
        // get state
        var isChecked = this.ShowColor.IsChecked ?? false;
        this.SourceBlock.Visibility = isChecked ? Visibility.Collapsed : Visibility.Visible;
        this.ColorBlock.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
    }
}
