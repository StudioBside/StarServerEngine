namespace Cs.Core.IoC.Detail
{
    using System;

    internal class ScopedFactory<TParent, TChild> : IScopedFactory
            where TParent : class
            where TChild : TParent, new()
    {
        public Type Type { get; } = typeof(TParent);

        public IScopedInstanceHolder CreateHolder()
        {
            return new Holder();
        }

        public sealed class Holder : IScopedInstanceHolder
        {
            private readonly Lazy<TChild> instance = new Lazy<TChild>(() => new TChild());

            public Type Type { get; } = typeof(TParent);

            public T GetInstance<T>() where T : class
            {
                if (this.instance.Value is not T result)
                {
                    throw new Exception($"invalid type. required:{typeof(T).Name} instance:{this.Type.Name}");
                }

                return result;
            }
        }
    }
}
