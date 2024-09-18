namespace Du.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

public interface ICollectionEditor
{
    void Edit<T>(IList<T> collection);
}
