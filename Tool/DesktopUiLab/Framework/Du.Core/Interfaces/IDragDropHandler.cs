namespace Du.Core.Interfaces;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// drag and drop을 처리하려는 vm에서 구현해야 하는 인터페이스.
/// 하나의 vm에서 여러 listview에 대한 drag를 처리해야 할 때는 listViewContext를 이용하여 구분할 수 있다.
/// </summary>
public interface IDragDropHandler
{
    /// <summary>
    /// drop을 처리한다.
    /// </summary>
    /// <param name="listViewContext">drag and drop을 수행한 listView의 DataContext.</param>
    /// <param name="selectedItems">대상 listView의 selectedItems.</param>
    /// <param name="targetContext">drop한 위치의 ListViewItem의 DataContext.</param>
    /// <returns>drop을 처리했으면 true, 그렇지 않으면 false.</returns>
    bool HandleDrop(object listViewContext, IList selectedItems, object targetContext);
}
