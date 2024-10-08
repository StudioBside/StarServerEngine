﻿namespace Cs.Core.IoC.Detail
{
    using System;

    internal interface ITypeFactory
    {
        Type Type { get; }
        T GetInstance<T>() where T : class;
    }
}
