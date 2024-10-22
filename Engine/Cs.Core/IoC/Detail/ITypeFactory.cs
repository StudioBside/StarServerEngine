namespace Cs.Core.IoC.Detail
{
    using System;

    internal interface ITypeFactory
    {
        Type Type { get; }
        string Key { get; }
        T GetInstance<T>() where T : class;
    }
}
