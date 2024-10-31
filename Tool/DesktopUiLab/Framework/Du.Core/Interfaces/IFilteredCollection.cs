namespace Du.Core.Interfaces;

using System.Collections;

/// <summary>
/// ICollectionView에 준하는 기능을 제공합니다.
/// ICollectionView는 CollectionViewSource를 통해 생성할 수 있으나 ViewModel에서 참조하지 않는 dll에 존재합니다.
/// ICollectionView : WindowsBase.dll / in namespace System.ComponentModel
/// CollectionViewSource : PresentationFramework.dll / in namespace System.Windows.Data.
/// </summary>
public interface IFilteredCollection
{
    IEnumerable List { get; }
    int SourceCount { get; }

    void Refresh(string searchKeyword);
}
