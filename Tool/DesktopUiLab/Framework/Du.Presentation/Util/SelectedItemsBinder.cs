namespace Du.Presentation.Util;

using System.Collections;
using System.Collections.Specialized;
using System.Windows.Controls;

public sealed class SelectedItemsBinder
{
    private readonly ListView listView;
    private readonly IList collection;

    public SelectedItemsBinder(ListView listView, IList collection)
    {
        this.listView = listView;
        this.collection = collection;

        this.listView.SelectedItems.Clear();

        foreach (var item in this.collection)
        {
            this.listView.SelectedItems.Add(item);
        }
    }

    public void Bind()
    {
        this.listView.SelectionChanged += this.ListView_SelectionChanged;

        if (this.collection is INotifyCollectionChanged)
        {
            var observable = (INotifyCollectionChanged)this.collection;
            observable.CollectionChanged += this.Collection_CollectionChanged;
        }
    }

    public void UnBind()
    {
        if (this.listView != null)
        {
            this.listView.SelectionChanged -= this.ListView_SelectionChanged;
        }

        if (this.collection != null && this.collection is INotifyCollectionChanged)
        {
            var observable = (INotifyCollectionChanged)this.collection;
            observable.CollectionChanged -= this.Collection_CollectionChanged;
        }
    }

    private void Collection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in e.NewItems ?? new object[0])
        {
            if (!this.listView.SelectedItems.Contains(item))
            {
                this.listView.SelectedItems.Add(item);
            }
        }

        foreach (var item in e.OldItems ?? new object[0])
        {
            this.listView.SelectedItems.Remove(item);
        }
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (var item in e.AddedItems ?? new object[0])
        {
            if (!this.collection.Contains(item))
            {
                this.collection.Add(item);
            }
        }

        foreach (var item in e.RemovedItems ?? new object[0])
        {
            this.collection.Remove(item);
        }
    }
}
