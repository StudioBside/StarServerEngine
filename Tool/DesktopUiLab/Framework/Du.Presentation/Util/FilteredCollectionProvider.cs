namespace Du.Presentation.Util;

using System.Collections;
using System.Windows.Data;
using Du.Core.Interfaces;

/// <summary>
/// ViewModel에서는 Windows.Base.dll에 존재하는 ICollectionView을 참조하지 않으므로 ICollectionView를 대체하는 기능을 제공합니다.
/// </summary>
public sealed class FilteredCollectionProvider : IFilteredCollectionProvider
{
    IFilteredCollection<T> IFilteredCollectionProvider.Build<T>(IEnumerable<T> collection)
    {
        IList listType = collection as IList ?? collection.ToArray();
        return new FilteredCollection<T>(listType);
    }

    private sealed class FilteredCollection<T>(IList collection) : IFilteredCollection<T>
    {
        private readonly ListCollectionView view = new(collection);

        public IEnumerable List => this.view;
        public IEnumerable<T> TypedList => this.view.Cast<T>();
        public int SourceCount => collection.Count;
        public int FilteredCount => this.view.Count;
        public Predicate<T>? Filter
        {
            get => this.view.Filter as Predicate<T>;
            set => this.view.Filter = Convert(value);
        }

        public void Refresh()
        {
            this.view.Refresh();
        }

        public void AddLiveFilteringProperty(string propertyName)
        {
            this.view.LiveFilteringProperties.Add(propertyName);
            this.view.IsLiveFiltering = true;
        }

        //// ----------------------------------------------------------------

        private static Predicate<object>? Convert(Predicate<T>? predicate)
        {
            if (predicate is null)
            {
                return null;
            }

            return e =>
            {
                if (e is not T item)
                {
                    return false;
                }

                return predicate(item);
            };
        }
    }
}
