namespace Du.Presentation.Behaviors;

using System.Windows;
using Du.Core.Interfaces;
using Du.Presentation.Util;
using Microsoft.Xaml.Behaviors;

public sealed class FileDropBehavior : Behavior<UIElement>
{
    protected override void OnAttached()
    {
        this.AssociatedObject.AllowDrop = true;
        this.AssociatedObject.DragEnter += this.AssociatedObject_DragEnter;
        this.AssociatedObject.Drop += this.AssociatedObject_Drop;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.DragEnter -= this.AssociatedObject_DragEnter;
        this.AssociatedObject.Drop -= this.AssociatedObject_Drop;
    }

    private void AssociatedObject_DragEnter(object sender, DragEventArgs e)
    {
        // 드래그한 데이터가 파일인지 확인
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy; // 드롭 가능
        }
        else
        {
            e.Effects = DragDropEffects.None; // 드롭 불가
        }
    }

    private void AssociatedObject_Drop(object sender, DragEventArgs e)
    {
        // 드래그한 데이터를 파일 배열로 가져옴
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return;
        }

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files == null || files.Length == 0)
        {
            return;
        }

        if (this.AssociatedObject.FindAncestorDataContext<IFileDropHandler>(out var handler))
        {
            handler.HandleDroppedFiles(files);
        }
    }
}
