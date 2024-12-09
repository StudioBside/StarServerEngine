namespace Cs.Dynamic;

using System;

// https://codingsolution.wordpress.com/2013/07/12/activator-createinstance-is-slow/
public static class Activator<T>
{
    static Activator()
    {
        CreateInstance = GetConstructorDelegate<T>();
    }

    public static Func<T> CreateInstance { get; private set; }

    private static Func<TBase> GetConstructorDelegate<TBase>()
    {
        return (Func<TBase>)TypeExt.GetConstructorDelegate(typeof(TBase), typeof(Func<TBase>));
    }
}
