namespace Cs.Core.IoC.Factory
{
    using System;
    using Cs.Core.IoC.Detail;

    internal class SingletonHolder<TInstance> : ITypeFactory
        where TInstance : class
    {
        private readonly TInstance instance;

        public SingletonHolder(TInstance instance)
        {
            this.instance = instance;
            this.Type = typeof(TInstance);
        }

        public bool IsScoped { get; } = false;
        public Type Type { get; }

        public T GetInstance<T>() where T : class
        {
            var result = this.instance as T;
            if (typeof(T) != this.Type || result is null)
            {
                throw new Exception($"invalid type. required:{typeof(T).Name} instance:{this.Type.Name}");
            }

            return result;
        }
    }
}
