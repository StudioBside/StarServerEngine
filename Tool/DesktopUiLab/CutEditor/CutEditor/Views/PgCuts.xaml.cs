namespace CutEditor.Views;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CutEditor.ViewModel;
using Du.Core.Util;
using ListViewItem = System.Windows.Controls.ListViewItem;

public sealed partial class PgCuts : Page
{
    public PgCuts()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService<VmCuts>();
    }

    private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        while (current != null && !(current is T))
        {
            current = VisualTreeHelper.GetParent(current);
        }

        return current as T;
    }

    private void CutList_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
        if (listViewItem == null)
        {
            return;
        }

        if (this.CutList.SelectedItems.Count == 0)
        {
            return;
        }

        var data = this.CutList.SelectedItems.Cast<object>().ToList();
        DragDrop.DoDragDrop(listViewItem, data, DragDropEffects.Move);
    }

    private void CutList_Drop(object sender, DragEventArgs e)
    {
        var target = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
        if (target == null)
        {
            return;
        }

        var dropTarget = (VmCut)target.DataContext;
        var items = this.CutList.SelectedItems.Cast<VmCut>().ToList();

        if (items is null)
        {
            return;
        }

        // ViewModel에서 아이템 위치 변경
        var vm = (VmCuts)this.DataContext;
        vm.MoveItems(items, dropTarget);
    }
}
