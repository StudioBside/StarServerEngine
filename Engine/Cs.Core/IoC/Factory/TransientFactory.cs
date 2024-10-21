namespace Cs.Core.IoC.Factory
{
    using System;
    using Cs.Core.IoC.Detail;

    internal class TransientFactory<TInstance> : ITypeFactory
         where TInstance : class
    {
        private readonly Func<TInstance> factory;

        public TransientFactory(Func<TInstance> factory)
        {
            this.factory = factory;
            this.Type = typeof(TInstance);
        }

        public Type Type { get; }

        public T GetInstance<T>() where T : class
        {
            var result = this.factory.Invoke() as T;
            if (typeof(T) != this.Type || result is null)
            {
                throw new Exception($"invalid type. required:{typeof(T).Name} instance:{this.Type.Name}");
            }

            return result;
        }
    }
}
