namespace CutEditor.Views.Preview;

using System.Windows;
using System.Windows.Controls;

public partial class PreviewUnitSlot : UserControl
{
    private static readonly DependencyProperty ChildContentProperty = DependencyProperty.Register(
        "ChildContent",
        typeof(UIElement),
        typeof(PreviewUnitSlot),
        new PropertyMetadata(null));

    public PreviewUnitSlot()
    {
        this.InitializeComponent();
    }

    public UIElement ChildContent
    {
        get => (UIElement)this.GetValue(ChildContentProperty);
        set => this.SetValue(ChildContentProperty, value);
    }
}
