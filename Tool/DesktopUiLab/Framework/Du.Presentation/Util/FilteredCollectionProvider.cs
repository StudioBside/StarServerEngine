namespace Du.Presentation.Util;

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using Du.Core.Interfaces;

public sealed class FilteredCollectionProvider : IFilteredCollectionProvider
{
    IFilteredCollection IFilteredCollectionProvider.Build<T>(IList<T> collection)
    {
        return new FilteredCollection<T>(collection);
    }

    private sealed class FilteredCollection<T> : IFilteredCollection
        where T : ISearchable
    {
        private readonly ListCollectionView view;

        public FilteredCollection(IList<T> collection)
        {
            this.view = new ListCollectionView(collection as IList);
        }

        public IEnumerable List => this.view;

        public void Refresh(string searchKeyword)
        {
            if (string.IsNullOrEmpty(searchKeyword))
            {
                this.view.Filter = null;
                return;
            }

            this.view.Filter = e =>
            {
                if (e is not T item)
                {
                    return false;
                }

                return item.IsTarget(searchKeyword);
            };
        }
    }
}
