namespace Cs.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExt
    {
        public static IEnumerable<TKey> GetDuplicatedKeys<TKey, TElement>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Where(e => e.Count() > 1).Select(e => e.Key);
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        {
            return source.Where(e => e != null).Select(e => e!);
        }
    }
}
