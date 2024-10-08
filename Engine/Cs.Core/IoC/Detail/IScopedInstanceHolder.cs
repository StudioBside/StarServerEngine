namespace Cs.Core.IoC.Detail
{
    using System;

    internal interface IScopedInstanceHolder
    {
        Type Type { get; }
        T GetInstance<T>() where T : class;
    }
}
