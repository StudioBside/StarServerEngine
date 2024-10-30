namespace Du.Presentation.Util;

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

public static class VisualTreeExt
{
    public static T? FindAncestor<T>(this DependencyObject current) where T : DependencyObject
    {
        if (current is Inline inline)
        {
            DependencyObject parent = inline.Parent;
            while (parent != null && parent is not TextBlock)
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            if (parent is null)
            {
                return null;
            }

            current = parent;
        }
         
        while (current != null && current is not T)
        {
            current = VisualTreeHelper.GetParent(current);
        }

        return current as T;
    }

    public static bool FindAncestor<T>(this object current, [NotNullWhen(true)] out T? ancestor) where T : DependencyObject
    {
        if (current is not DependencyObject dependencyObject)
        {
            ancestor = null;
            return false;
        }

        ancestor = dependencyObject.FindAncestor<T>();
        return ancestor != null;
    }

    public static bool FindAncestorDataContext<T>(this object current, [NotNullWhen(true)] out T? dataContext) where T : class
    {
        if (current is not DependencyObject dependencyObject)
        {
            dataContext = null;
            return false;
        }

        DependencyObject? currentObject = dependencyObject;
        while (currentObject != null)
        {
            if (currentObject is FrameworkElement fe && fe.DataContext is T result)
            {
                dataContext = result;
                return true;
            }

            currentObject = VisualTreeHelper.GetParent(currentObject);
        }

        dataContext = null;
        return false;
    }
}
