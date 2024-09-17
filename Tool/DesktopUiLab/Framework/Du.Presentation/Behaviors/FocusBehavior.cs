namespace Du.Presentation.Behaviors;

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

public class FocusBehavior : Behavior<TextBox>
{
    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.Register(
            "IsFocused",
            typeof(bool),
            typeof(FocusBehavior),
            new PropertyMetadata(false, OnIsFocusedChanged));

    public bool IsFocused
    {
        get => (bool)this.GetValue(IsFocusedProperty);
        set => this.SetValue(IsFocusedProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (this.IsFocused)
        {
            this.AssociatedObject.Focus();
        }
    }

    private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FocusBehavior behavior && behavior.AssociatedObject != null && (bool)e.NewValue)
        {
            behavior.AssociatedObject.Focus();
        }
    }

/*
<Window x:Class="YourNamespace.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
        xmlns:local="clr-namespace:YourNamespace"
        Title="MainWindow" Height="350" Width="525">
    <Grid>
        <TextBox Width="200" Height="30">
            <i:Interaction.Behaviors>
                <local:FocusBehavior IsFocused="True" />
            </i:Interaction.Behaviors>
        </TextBox>
    </Grid>
</Window>
*/
}
