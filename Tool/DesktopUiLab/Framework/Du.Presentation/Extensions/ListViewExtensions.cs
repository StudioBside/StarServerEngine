namespace Du.Presentation.Extensions;

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Du.Presentation.Util;

public sealed class ListViewExtensions
{
    private static readonly DependencyProperty SelectedValueBinderProperty = DependencyProperty.RegisterAttached(
        "SelectedValueBinder",
        typeof(SelectedItemsBinder),
        typeof(ListViewExtensions));

    private static readonly DependencyProperty SelectedValuesProperty = DependencyProperty.RegisterAttached(
        "SelectedValues",
        typeof(IList),
        typeof(ListViewExtensions),
        new FrameworkPropertyMetadata(null, OnSelectedValuesChanged));

    public static void SetSelectedValues(Selector elementName, IEnumerable value)
    {
        elementName.SetValue(SelectedValuesProperty, value);
    }

    public static IEnumerable GetSelectedValues(Selector elementName)
    {
        return (IEnumerable)elementName.GetValue(SelectedValuesProperty);
    }

    private static SelectedItemsBinder GetSelectedValueBinder(DependencyObject obj)
    {
        return (SelectedItemsBinder)obj.GetValue(SelectedValueBinderProperty);
    }

    //// ----------------------------------------------------------------------------------------------

    private static void SetSelectedValueBinder(DependencyObject obj, SelectedItemsBinder items)
    {
        obj.SetValue(SelectedValueBinderProperty, items);
    }

    private static void OnSelectedValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs value)
    {
        var oldBinder = GetSelectedValueBinder(o);
        oldBinder?.UnBind();

        SetSelectedValueBinder(o, new SelectedItemsBinder((ListView)o, (IList)value.NewValue));
        GetSelectedValueBinder(o).Bind();
    }
}