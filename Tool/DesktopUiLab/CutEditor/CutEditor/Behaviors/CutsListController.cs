namespace CutEditor.Behaviors;

using System.Windows.Controls;
using CutEditor.Model.Interfaces;
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

    //// --------------------------------------------------------------------
    
    private void ScrollIntoViewImpl(int index)
    {
        this.AssociatedObject.ScrollIntoView(this.AssociatedObject.Items[index]);
    }
}
