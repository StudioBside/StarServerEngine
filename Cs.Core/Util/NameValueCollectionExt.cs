namespace Cs.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;

    public static class NameValueCollectionExt
    {
        public static bool? GetBool(this NameValueCollection self, string name)
        {
            if (bool.TryParse(self[name], out var result))
            {
                return result;
            }

            return null;
        }

        public static string GetString(this NameValueCollection self, string name, string @default)
        {
            var value = self.Get(name);
            if (string.IsNullOrEmpty(value))
            {
                return @default;
            }

            return value;
        }

        public static int GetInt(this NameValueCollection self, string name, int @default)
        {
            var value = self.Get(name);
            if (string.IsNullOrEmpty(value))
            {
                return @default;
            }

            if (!int.TryParse(value, out int result))
            {
                return @default;
            }

            return result;
        }

        public static long GetLong(this NameValueCollection self, string name, int @default)
        {
            var value = self.Get(name);
            if (string.IsNullOrEmpty(value))
            {
                return @default;
            }

            if (!long.TryParse(value, out long result))
            {
                return @default;
            }

            return result;
        }

        public static bool TryGetValue(this NameValueCollection self, string name, [NotNullWhen(returnValue: true)] out string? value)
        {
            value = self.Get(name);
            return value != null;
        }

        public static DateTime TryGetDateTime(this NameValueCollection self, string name, DateTime @defaultValue)
        {
            var value = self.Get(name);
            if (string.IsNullOrEmpty(value))
            {
                return @defaultValue;
            }

            if (!DateTime.TryParse(value, out DateTime result))
            {
                return defaultValue;
            }

            return result;
        }

        public static IEnumerable<KeyValuePair<string, string>> Pairs(this NameValueCollection self)
        {
            foreach (string? name in self)
            {
                if (name == null)
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(name, self.Get(name) ?? string.Empty);
            }
        }
    }
}