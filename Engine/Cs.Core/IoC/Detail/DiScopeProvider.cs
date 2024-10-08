#nullable enable

namespace Cs.Core.IoC.Detail
{
    using System;
    using System.Collections.Generic;

    internal sealed class DiScopeProvider : IDiScopeProvider
    {
        private readonly List<IScopedInstanceHolder> instances = new();

        public DiScopeProvider(IEnumerable<IScopedInstanceHolder> instances)
        {
            this.instances.AddRange(instances);
        }

        public void Dispose()
        {
            this.instances.Clear();
        }

        public T GetInstance<T>() where T : class
        {
            var type = typeof(T);
            var holder = this.instances.Find(e => e.Type == type);
            if (holder is null)
            {
                throw new Exception($"instance not found. type:{type.Name}");
            }

            return holder.GetInstance<T>();
        }
    }
}
