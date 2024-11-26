namespace Du.Presentation.Util;

using System.Collections;
using System.Windows.Data;
using Du.Core.Interfaces;
using Shared.Interfaces;

/// <summary>
/// ViewModel에서는 Windows.Base.dll에 존재하는 ICollectionView을 참조하지 않으므로 ICollectionView를 대체하는 기능을 제공합니다.
/// </summary>
public sealed class SearchableCollectionProvider : ISearchableCollectionProvider
{
    ISearchableCollection<T> ISearchableCollectionProvider.Build<T>(IEnumerable<T> collection)
    {
        IList listType = collection as IList ?? collection.ToArray();
        return new SearchableCollection<T>(listType);
    }

    private sealed class SearchableCollection<T> : ISearchableCollection<T>
        where T : ISearchable
    {
        private readonly IList source;
        private readonly ListCollectionView view;
        private string searchKeyword = string.Empty;
        private Predicate<T>? subFilter;

        public SearchableCollection(IList source)
        {
            this.source = source;
            this.view = new ListCollectionView(source);
            this.view.Filter = this.Filter;
        }

        public IEnumerable List => this.view;
        public IEnumerable<T> TypedList => this.view.Cast<T>();
        public int SourceCount => this.source.Count;
        public int FilteredCount => this.view.Count;

        public void SetSubFilter(Predicate<T>? filter)
        {
            this.subFilter = filter;
            this.view.Refresh();
        }

        public void Refresh()
        {
            this.view.Refresh();
        }

        public void Refresh(string searchKeyword)
        {
            this.searchKeyword = searchKeyword;
            this.view.Refresh();
        }

        public void AddGroupDescription(string propertyName)
        {
            this.view.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
        }

        //// -----------------------------------------------------------------------------------------------

        private bool Filter(object obj)
        {
            if (obj is not T item)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.searchKeyword) && this.subFilter is null)
            {
                return true;
            }

            if (this.subFilter is not null && this.subFilter(item) == false)
            {
                return false;
            }

            return item.IsTarget(this.searchKeyword);
        }
    }
}
