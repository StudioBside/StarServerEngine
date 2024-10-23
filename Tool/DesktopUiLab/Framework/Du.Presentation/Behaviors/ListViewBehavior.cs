namespace Du.Presentation.Behaviors;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cs.Logging;
using Du.Core.Interfaces;
using Du.Presentation.Util;
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
        this.AssociatedObject.PreviewMouseLeftButtonDown += this.AssociatedObject_PreviewMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.PreviewMouseMove -= this.AssociatedObject_PreviewMouseMove;
        this.AssociatedObject.Drop -= this.AssociatedObject_Drop;
        this.AssociatedObject.PreviewMouseLeftButtonDown -= this.AssociatedObject_PreviewMouseLeftButtonDown;
    }

    private static void OnReorderByDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed ||
            e.OriginalSource.FindAncestor<ListViewItem>(out var listViewItem) == false)
        {
            return;
        }

        if (this.AssociatedObject.SelectedItems.Count == 0)
        {
            return;
        }

        var data = this.AssociatedObject.SelectedItems.Cast<object>().ToList();
        try
        {
            DragDrop.DoDragDrop(listViewItem, data, DragDropEffects.Move);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }

    private void AssociatedObject_Drop(object sender, DragEventArgs e)
    {
        if (this.AssociatedObject.FindAncestor<Page>(out var page) == false ||
            page.DataContext is not IDragDropHandler handler)
        {
            Log.Warn($"DataContext is not IDragDropHandler. dataContext:{this.AssociatedObject.DataContext}");
            return;
        }

        if (e.OriginalSource.FindAncestor<ListViewItem>(out var target) == false)
        {
            return;
        }

        handler.HandleDrop(this.AssociatedObject.DataContext, this.AssociatedObject.SelectedItems, target.DataContext);
        e.Handled = true;
    }

    private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var item = ItemsControl.ContainerFromElement(this.AssociatedObject, e.OriginalSource as DependencyObject) as ListViewItem;

        if (item != null && Keyboard.Modifiers == ModifierKeys.None)
        {
            if (item.IsSelected)
            {
                // 이미 선택된 항목을 다시 클릭해도 선택 상태를 유지
                e.Handled = true;
            }
        }
    }
}
