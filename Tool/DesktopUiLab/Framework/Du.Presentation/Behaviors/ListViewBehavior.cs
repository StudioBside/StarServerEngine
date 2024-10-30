namespace Du.Presentation.Behaviors;

using System;
using System.Collections;
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
    private ToolTip? toolTip;

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
        this.AssociatedObject.DragOver += this.AssociatedObject_DragOver;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.PreviewMouseLeftButtonDown -= this.AssociatedObject_PreviewMouseLeftButtonDown;
        this.AssociatedObject.PreviewMouseMove -= this.AssociatedObject_PreviewMouseMove;
        this.AssociatedObject.Drop -= this.AssociatedObject_Drop;
        this.AssociatedObject.DragOver -= this.AssociatedObject_DragOver;
    }

    private static void OnReorderByDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private void AssociatedObject_DragOver(object sender, DragEventArgs e)
    {
        ////// 커서 위치 가져오기
        ////var position = e.GetPosition(this.AssociatedObject);

        ////// 정보 표시를 위한 툴팁 생성
        ////var toolTip = new ToolTip
        ////{
        ////    Content = $"Uid: 드래그 추가정보",
        ////    IsOpen = true,
        ////    Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse,
        ////};

        ////// 툴팁을 커서 위치에 표시
        ////ToolTipService.SetToolTip(this, this.toolTip);
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

        if (e.OriginalSource.FindAncestor<ListViewItem>(out var clickedItem) && // 아이템 클릭 했을 때
            clickedItem.IsSelected && // 현재 선택된 상태이고
            Keyboard.Modifiers.HasFlag(ModifierKeys.Control) == false && // ctrl 누른 상태 아니고
            Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) == false && // shift 누른 상태 아니고
            this.AssociatedObject.SelectedItems.Count > 1) // 선택된 아이템이 여러개일 때
        {
            e.Handled = true; // 이벤트를 무시해서 지금 선택된 리스트가 리셋되는 것을 막아준다.
            return;
        }
    }

    private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed && this.toolTip is not null && this.toolTip.IsOpen)
        {
            this.toolTip.IsOpen = false;
        }

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
        if (this.toolTip is null)
        {
            this.toolTip = new ToolTip
            {
                Content = $"드래그 추가정보",
                IsOpen = true,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse,
            };

            // 툴팁을 커서 위치에 표시
            ToolTipService.SetToolTip(this, this.toolTip);
        }

        try
        {
            this.toolTip.Content = $"{this.AssociatedObject.SelectedItems.Count}개 항목 위치 이동";
            this.toolTip.IsOpen = true;

            DragDrop.DoDragDrop(dragStartItem, selectedItems, DragDropEffects.Move);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }

    private void AssociatedObject_Drop(object sender, DragEventArgs e)
    {
        if (this.toolTip is not null && this.toolTip.IsOpen)
        {
            this.toolTip.IsOpen = false;
        }

        if (e.OriginalSource.FindAncestor<ListViewItem>(out var dragStopItem) == false)
        {
            return;
        }

        ////handler.HandleDrop(this.AssociatedObject.DataContext, this.AssociatedObject.SelectedItems, dragStopItem.DataContext);

        var list = this.AssociatedObject.ItemsSource as IList ?? throw new Exception("DataContext is not IList.");
        var selected = new object[this.AssociatedObject.SelectedItems.Count];
        this.AssociatedObject.SelectedItems.CopyTo(selected, 0);

        var dropIndex = list.IndexOf(dragStopItem.DataContext);
        var itemsIndex = selected.Select(list.IndexOf).OrderByDescending(i => i).ToList();
        if (itemsIndex.Count == 0)
        {
            return;
        }

        // drop 대상이 items에 속해있으면 에러
        if (itemsIndex.Contains(dropIndex))
        {
            //Log.Error("drop target is in the moving items.");
            return;
        }

        var indices = itemsIndex.Count == 1
            ? itemsIndex[0].ToString()
            : string.Join(", ", itemsIndex.OrderBy(e => e));

        // itemsIndex가 연속적이지 않으면 에러
        if (itemsIndex.Count > 1)
        {
            var min = itemsIndex.Min();
            var max = itemsIndex.Max();
            if (max - min + 1 != itemsIndex.Count)
            {
                Log.Warn($"위치를 이동할 아이템은 연속적으로 선택해야 합니다. 현재 선택 인덱스:{indices}");
                return;
            }
        }

        Log.Info($"위치 조정: {indices} -> {dropIndex}");

        var movingItems = new List<object>();
        foreach (var i in itemsIndex)
        {
            var movingItem = list[i];
            if (movingItem == null)
            {
                continue;
            }

            movingItems.Add(movingItem);
            list.RemoveAt(i);
        }

        // 더 아래로 내리는 경우는 목적지 인덱스가 바뀔테니 재계산 필요
        if (dropIndex > itemsIndex.Max())
        {
            dropIndex -= itemsIndex.Count - 1;
        }

        foreach (var item in movingItems)
        {
            list.Insert(dropIndex, item);
        }

        e.Handled = true;
    }
}
