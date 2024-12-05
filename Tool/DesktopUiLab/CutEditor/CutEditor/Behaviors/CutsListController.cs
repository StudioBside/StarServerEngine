namespace CutEditor.Behaviors;

using System;
using System.Windows.Controls;
using CutEditor.Model.Interfaces;
using Du.Presentation.Util;
using Microsoft.Xaml.Behaviors;

internal sealed class CutsListController : Behavior<ListView>,
    ICutsListController
{
    private static CutsListController lastInstance = new();

    public CutsListController()
    {
        lastInstance = this;
    }

    public static ICutsListController LastInstance => lastInstance;

    void ICutsListController.ScrollIntoView(int index)
    {
        lastInstance.ScrollIntoViewImpl(index);
    }

    void ICutsListController.FocusElement(int index)
    {
        lastInstance.FocusElementImpl(index);
    }

    //// --------------------------------------------------------------------

    private void ScrollIntoViewImpl(int index)
    {
        this.AssociatedObject.ScrollIntoView(this.AssociatedObject.Items[index]);
    }

    private void FocusElementImpl(int index)
    {
        if (index >= 0 && index < this.AssociatedObject.Items.Count)
        {
            var item = this.AssociatedObject.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            if (item != null)
            {
                item.Focus();
            }
        }
    }
}
