#nullable enable

namespace Cs.Core.IoC
{
    using System;

    public interface IDiScopeProvider : IDisposable
    {
        T GetInstance<T>() where T : class;
    }
}
