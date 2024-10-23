#nullable enable

namespace Cs.Core.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Cs.Core.IoC.Detail;
    using Cs.Core.IoC.Factory;

    public sealed class DiContainer
    {
        private readonly List<ITypeFactory> instances = new();
        private readonly List<IScopedFactory> scopedFactories = new();

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

        public DiContainer AddSingleton<T>(string key, T instance) where T : class
        {
            //// note: thread un-safe.

            var type = typeof(T);
            if (this.TryGet(key, type, out _))
            {
                throw new Exception($"instance already exist. key:{key} type:{type.Name}");
            }

            this.instances.Add(new SingletonHolder<T>(key, instance));
            return this;
        }

        public DiContainer AddTransient<TParent, TChild>()
            where TParent : class
            where TChild : TParent, new()
        {
            //// note: thread un-safe.

            var type = typeof(TParent);
            if (this.TryGet(type, out _))
            {
                throw new Exception($"instance already exist. type:{type.Name}");
            }

            this.instances.Add(new TransientFactory<TParent>(() => new TChild()));
            return this;
        }

        public DiContainer AddTransient<TParent, TChild>(string key)
            where TParent : class
            where TChild : TParent, new()
        {
            //// note: thread un-safe.

            var type = typeof(TParent);
            if (this.TryGet(key, type, out _))
            {
                throw new Exception($"instance already exist. key:{key} type:{type.Name}");
            }

            this.instances.Add(new TransientFactory<TParent>(key, () => new TChild()));
            return this;
        }

        public DiContainer AddScoped<TParent, TChild>()
            where TParent : class
            where TChild : TParent, new()
        {
            //// note: thread un-safe.

            var type = typeof(TParent);
            if (this.scopedFactories.Find(e => e.Type == type) != null)
            {
                throw new Exception($"instance already exist. type:{type.Name}");
            }

            this.scopedFactories.Add(new ScopedFactory<TParent, TChild>());
            return this;
        }

        public DiContainer UpdateTransient<T>(Func<T> factory) where T : class
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

        public DiContainer UpdateTransient<T>(string key, Func<T> factory) where T : class
        {
            //// note: thread un-safe.

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("invalid useage: key is empty.");
            }

            var type = typeof(T);
            for (int i = 0; i < this.instances.Count; ++i)
            {
                if (this.instances[i].Type == type && this.instances[i].Key.Equals(key))
                {
                    this.instances[i] = new TransientFactory<T>(key, factory);
                    return this;
                }
            }

            this.instances.Add(new TransientFactory<T>(key, factory));
            return this;
        }

        public T GetInstance<T>() where T : class
        {
            if (this.TryGet(typeof(T), out var instance) == false)
            {
                throw new Exception($"instance not exist. #instance:{this.instances.Count} type:{typeof(T).Name}");
            }

            return instance.GetInstance<T>();
        }

        public T GetInstance<T>(string key) where T : class
        {
            if (this.TryGet(key, typeof(T), out var instance) == false)
            {
                throw new Exception($"instance not exist. #instance:{this.instances.Count} type:{typeof(T).Name}");
            }

            return instance.GetInstance<T>();
        }

        public bool TryGetInstance<T>([MaybeNullWhen(false)] out T instance) where T : class
        {
            if (this.TryGet(typeof(T), out var factory))
            {
                instance = factory.GetInstance<T>();
                return true;
            }

            instance = default;
            return false;
        }

        public bool TryGetInstance<T>(string key, [MaybeNullWhen(false)] out T instance) where T : class
        {
            if (this.TryGet(key, typeof(T), out var factory))
            {
                instance = factory.GetInstance<T>();
                return true;
            }

            instance = default;
            return false;
        }

        public IDiScopeProvider CreateScope()
        {
            return new DiScopeProvider(this.scopedFactories.Select(e => e.CreateHolder()));
        }

        //// ---------------------------------------------------------------------------------------------------

        private bool TryGet(Type type, [MaybeNullWhen(false)] out ITypeFactory instance)
        {
            foreach (var data in this.instances)
            {
                if (data.Type == type && string.IsNullOrEmpty(data.Key))
                {
                    instance = data;
                    return true;
                }
            }

            instance = default;
            return false;
        }

        private bool TryGet(string key, Type type, [MaybeNullWhen(false)] out ITypeFactory instance)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("invalid useage: key is empty.");
            }

            foreach (var data in this.instances)
            {
                if (data.Type == type && data.Key.Equals(key))
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
