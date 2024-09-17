namespace Du.Presentation.Extensions;

using System.Windows;
using System.Windows.Controls;

public static class FocusExtension
{
    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool),
            typeof(FocusExtension),
            new PropertyMetadata(false, OnIsFocusedPropertyChanged));

    // select all text when focused
    public static readonly DependencyProperty SelectAllTextProperty =
        DependencyProperty.RegisterAttached(
            "SelectAllText",
            typeof(bool),
            typeof(FocusExtension),
            new PropertyMetadata(false, OnSelectAllTextPropertyChanged));

    public static bool GetIsFocused(DependencyObject obj) => (bool)obj.GetValue(IsFocusedProperty);
    public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

    public static bool GetSelectAllText(DependencyObject obj) => (bool)obj.GetValue(SelectAllTextProperty);
    public static void SetSelectAllText(DependencyObject obj, bool value) => obj.SetValue(SelectAllTextProperty, value);

    private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element && (bool)e.NewValue)
        {
            element.Focus();
        }
    }

    private static void OnSelectAllTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox && (bool)e.NewValue)
        {
            if (textBox.IsLoaded)
            {
                textBox.SelectAll();
            }
            else
            {
                textBox.Loaded += (sender, e) => textBox.SelectAll();
            }
        }
    }

    /*
    <Window x:Class="YourNamespace.MainWindow"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:local="clr-namespace:YourNamespace"
            Title="MainWindow" Height="350" Width="525">
        <Grid>
            <TextBox local:FocusExtension.IsFocused="True" Width="200" Height="30" />
        </Grid>
    </Window>
    */
}
