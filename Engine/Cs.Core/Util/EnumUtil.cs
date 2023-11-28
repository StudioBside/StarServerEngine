namespace Cs.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumUtil<T> where T : Enum
    {
        private static string[]? nameCache = null;

        public static int Count
        {
            get
            {
                nameCache ??= Enum.GetNames(typeof(T));
                return nameCache.Length;
            }
        }

        public static IEnumerable<T> GetValues()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<string> GetDescriptions()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(e => e.GetDescription());
        }

        public static bool IsDefined(T value)
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}
