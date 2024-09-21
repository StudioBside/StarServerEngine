#nullable enable

namespace Cs.Core.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Cs.Core.IoC.Detail;
    using Cs.Core.IoC.Factory;

    public sealed class DiContainer
    {
        private readonly List<ITypeFactory> instances = new();

        public static DiContainer Instance => Singleton<DiContainer>.Instance;

        public DiContainer AddSingleton<T>(T instance) where T : class
        {
            //// note: thread un-safe.

            var type = typeof(T);
            if (this.TryGet(type, out _))
            {
                throw new Exception($"instance already exist. type:{type.Name}");
            }

            this.instances.Add(new SingletonHolder<T>(instance));
            return this;
        }

        public DiContainer UpdateSingleton<T>(T instance) where T : class
        {
            //// note: thread un-safe.

            var type = typeof(T);
            for (int i = 0; i < this.instances.Count; ++i)
            {
                if (this.instances[i].Type == type)
                {
                    this.instances[i] = new SingletonHolder<T>(instance);
                    return this;
                }
            }

            this.instances.Add(new SingletonHolder<T>(instance));
            return this;
        }

        public DiContainer AddTransient<T>(Func<object?, T> factory) where T : class
        {
            //// note: thread un-safe.

            var type = typeof(T);
            if (this.TryGet(type, out _))
            {
                throw new Exception($"instance already exist. type:{type.Name}");
            }

            this.instances.Add(new TransientFactory<T>(factory));
            return this;
        }

        public DiContainer UpdateTransient<T>(Func<object?, T> factory) where T : class
        {
            //// note: thread un-safe.

            var type = typeof(T);
            for (int i = 0; i < this.instances.Count; ++i)
            {
                if (this.instances[i].Type == type)
                {
                    this.instances[i] = new TransientFactory<T>(factory);
                    return this;
                }
            }

            this.instances.Add(new TransientFactory<T>(factory));
            return this;
        }

        public T GetInstance<T>(object? param) where T : class
        {
            if (this.TryGet(typeof(T), out var instance) == false)
            {
                throw new Exception($"instance not exist. #instance:{this.instances.Count} type:{typeof(T).Name}");
            }

            return instance.GetInstance<T>(param);
        }

        //// ---------------------------------------------------------------------------------------------------

        private bool TryGet(Type type, [MaybeNullWhen(false)] out ITypeFactory instance)
        {
            foreach (var data in this.instances)
            {
                if (data.Type == type)
                {
                    instance = data;
                    return true;
                }
            }

            instance = default;
            return false;
        }
    }
}
