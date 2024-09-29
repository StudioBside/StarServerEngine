namespace Du.Presentation.Util;

using System;
using System.Collections;
using System.Windows.Data;
using Du.Core.Interfaces;

public sealed class FilteredCollectionProvider : IFilteredCollectionProvider
{
    IEnumerable IFilteredCollectionProvider.Build<T>(IList<T> collection, Predicate<T> filter)
    {
        var sourceView = new CollectionViewSource { Source = collection }.View;
        sourceView.Filter = e =>
        {
            if (e is not T item)
            {
                return false;
            }

            return filter(item);
        };

        return sourceView;
    }
}
