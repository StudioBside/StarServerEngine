namespace Cs.Legacy.Dynamic
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    // https://codingsolution.wordpress.com/2013/07/12/activator-createinstance-is-slow/
    public static class TypeExt
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Constructors = new ConcurrentDictionary<Type, Func<object>>();

        public static object CreateInstance(this Type type)
        {
            if (!Constructors.TryGetValue(type, out Func<object> constructor))
            {
                constructor = type.GetConstructorDelegate();
                Constructors.TryAdd(type, constructor);
            }

            return constructor();
        }

        internal static Delegate GetConstructorDelegate(Type type, Type delegateType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (delegateType == null)
            {
                throw new ArgumentNullException(nameof(delegateType));
            }

            Type[] genericArguments = delegateType.GetGenericArguments();
            Type[] argTypes = genericArguments.Length > 1
                ? genericArguments.Take(genericArguments.Length - 1).ToArray()
                : Type.EmptyTypes;

            ConstructorInfo constructor = type.GetConstructor(argTypes);
            if (constructor == null)
            {
                if (argTypes.Length == 0)
                {
                    throw new InvalidProgramException($"Type '{type.Name}' doesn't have a parameterless constructor.");
                }

                throw new InvalidProgramException($"Type '{type.Name}' doesn't have the requested constructor.");
            }

            var dynamicMethod = new DynamicMethod("DM$_" + type.Name, type, argTypes, type);
            ILGenerator ilGen = dynamicMethod.GetILGenerator();
            for (int i = 0; i < argTypes.Length; i++)
            {
                ilGen.Emit(OpCodes.Ldarg, i);
            }

            ilGen.Emit(OpCodes.Newobj, constructor);
            ilGen.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(delegateType);
        }

        private static Func<object> GetConstructorDelegate(this Type type)
        {
            return (Func<object>)GetConstructorDelegate(type, typeof(Func<object>));
        }
    }
}
