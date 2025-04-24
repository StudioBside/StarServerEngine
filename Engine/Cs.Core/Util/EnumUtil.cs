namespace Cs.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumUtil<T> where T : Enum
    {
        private static readonly T[] Values = (T[])Enum.GetValues(typeof(T));

        public static int Count => Values.Length;

        public static IEnumerable<T> GetValues() => Values;

        public static IEnumerable<string> GetDescriptions()
        {
            return Values.Select(e => e.GetDescription());
        }

        public static bool IsDefined(T value)
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}
