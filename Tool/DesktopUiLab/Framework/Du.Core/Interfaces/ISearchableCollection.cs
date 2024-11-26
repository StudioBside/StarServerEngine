namespace Du.Core.Interfaces;

using System.Collections;
using Shared.Interfaces;

/// <summary>
/// ICollectionView에 준하는 기능을 제공합니다.
/// ICollectionView는 CollectionViewSource를 통해 생성할 수 있으나 ViewModel에서 참조하지 않는 dll에 존재합니다.
/// ICollectionView : WindowsBase.dll / in namespace System.ComponentModel
/// CollectionViewSource : PresentationFramework.dll / in namespace System.Windows.Data.
/// </summary>
/// <typeparam name="T">필터링할 대상의 타입입니다.</typeparam>
public interface ISearchableCollection<T> where T : ISearchable
{
    IEnumerable List { get; }
    IEnumerable<T> TypedList { get; }
    int SourceCount { get; }
    int FilteredCount { get; }

    void Refresh(string searchKeyword);
    void Refresh();

    void AddGroupDescription(string propertyName);
    void SetSubFilter(Predicate<T>? filter);
}
