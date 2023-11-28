namespace Cs.Core
{
    using System.Threading;

    public abstract class PerThreadSingleton<T> where T : new()
    {
        private static readonly ThreadLocal<T> Tls = new ThreadLocal<T>(() => new T());
        public static T Instance => Tls.Value!;
    }
}
