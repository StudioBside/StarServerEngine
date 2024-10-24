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

public sealed class ListViewBehavior : Behavior<ListView>
{
    public static readonly DependencyProperty ReorderByDragDropProperty =
          DependencyProperty.Register(
              "ReorderByDragDrop",
              typeof(bool),
              typeof(ListViewBehavior),
              new PropertyMetadata(false, OnReorderByDragDropChanged));

    private Point cursorStartPos;

    public bool ReorderByDragDrop
    {
        get => (bool)this.GetValue(ReorderByDragDropProperty);
        set => this.SetValue(ReorderByDragDropProperty, value);
    }

    protected override void OnAttached()
    {
        this.AssociatedObject.PreviewMouseLeftButtonDown += this.AssociatedObject_PreviewMouseLeftButtonDown;
        this.AssociatedObject.PreviewMouseMove += this.AssociatedObject_PreviewMouseMove;
        this.AssociatedObject.Drop += this.AssociatedObject_Drop;
        this.AssociatedObject.Initialized += this.AssociatedObject_Initialized;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.PreviewMouseLeftButtonDown -= this.AssociatedObject_PreviewMouseLeftButtonDown;
        this.AssociatedObject.PreviewMouseMove -= this.AssociatedObject_PreviewMouseMove;
        this.AssociatedObject.Drop -= this.AssociatedObject_Drop;
        this.AssociatedObject.Initialized -= this.AssociatedObject_Initialized;
    }

    private static void OnReorderByDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private void AssociatedObject_Initialized(object? sender, EventArgs e)
    {
        var style = this.AssociatedObject.ItemContainerStyle ?? new Style(typeof(ListViewItem));

        var eventSetter = new EventSetter(
            UIElement.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(this.ListItem_PreviewMouseLeftButtonDown));

        style.Setters.Add(eventSetter);

        // 스타일을 다시 설정
        this.AssociatedObject.ItemContainerStyle = style;
    }

    private void ListItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Log.Debug($"item_PreviewMouseLeftButtonDown: {sender.GetType()}");

        if (e.OriginalSource.FindAncestor<ListViewItem>(out var clickedItem) == false)
        {
            return;
        }

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) ||
            Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ||
            this.AssociatedObject.SelectedItems.Count == 1)
        {
            return;
        }

        if (clickedItem.IsSelected)
        {
            e.Handled = true;
        }
    }

    private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.cursorStartPos = e.GetPosition(relativeTo: null);
    }

    private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || // 마우스 버튼이 눌려있지 않음
            e.OriginalSource.FindAncestor<ListViewItem>(out var dragStartItem) == false || // 대상 아이템을 찾을 수 없음
            this.AssociatedObject.SelectedItems.Count == 0) // 선택된 아이템이 없음
        {
            return;
        }

        // 마우스가 현재 선택된 아이템 위에 있지 않으면 리턴
        if (this.AssociatedObject.SelectedItems.Contains(dragStartItem.DataContext) == false)
        {
            return;
        }

        Point currentCursorPos = e.GetPosition(relativeTo: null);
        var cursorVector = this.cursorStartPos - currentCursorPos;
        if (cursorVector.Length < 10)
        {
            // 마우스가 일정 범위 움직여야만 드래그 시작
            return;
        }

        var selectedItems = this.AssociatedObject.SelectedItems.Cast<object>().ToList();
        try
        {
            DragDrop.DoDragDrop(dragStartItem, selectedItems, DragDropEffects.Move);
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

        if (e.OriginalSource.FindAncestor<ListViewItem>(out var dragStopItem) == false)
        {
            return;
        }

        handler.HandleDrop(this.AssociatedObject.DataContext, this.AssociatedObject.SelectedItems, dragStopItem.DataContext);
        e.Handled = true;
    }
}
