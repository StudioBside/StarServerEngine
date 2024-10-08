namespace Cs.Core.IoC.Detail
{
    using System;

    internal interface IScopedFactory
    {
        Type Type { get; }
        IScopedInstanceHolder CreateHolder();
    }
}
