namespace Du.Presentation.Behaviors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

public class ListViewBehavior : Behavior<ListView>
{
    public static readonly DependencyProperty ReorderByDragDropProperty =
          DependencyProperty.Register(
              "ReorderByDragDrop",
              typeof(bool),
              typeof(ListViewBehavior),
              new PropertyMetadata(false, OnReorderByDragDropChanged));

    public bool ReorderByDragDrop
    {
        get => (bool)this.GetValue(ReorderByDragDropProperty);
        set => this.SetValue(ReorderByDragDropProperty, value);
    }

    protected override void OnAttached()
    {
        this.AssociatedObject.PreviewMouseMove += this.AssociatedObject_PreviewMouseMove;
        this.AssociatedObject.Drop += this.AssociatedObject_Drop;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.PreviewMouseMove -= this.AssociatedObject_PreviewMouseMove;
        this.AssociatedObject.Drop -= this.AssociatedObject_Drop;
    }

    private static void OnReorderByDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void AssociatedObject_Drop(object sender, DragEventArgs e)
    {
        throw new NotImplementedException();
    }
}
