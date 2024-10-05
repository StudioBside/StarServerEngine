namespace Du.Presentation.Util;

using System.Collections;
using System.Windows.Data;
using Du.Core.Interfaces;
using Shared.Interfaces;

public sealed class FilteredCollectionProvider : IFilteredCollectionProvider
{
    IFilteredCollection IFilteredCollectionProvider.Build<T>(IList<T> collection)
    {
        return new FilteredCollection<T>(collection);
    }

    private sealed class FilteredCollection<T>(IList<T> collection) : IFilteredCollection
        where T : ISearchable
    {
        private readonly ListCollectionView view = new(collection as IList);

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
