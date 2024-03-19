namespace Cs.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using Cs.Logging;
    using Newtonsoft.Json.Linq;

    public static class JTokenExt
    {
        public static string GetString(this JToken self, string key)
        {
            return GetString(self, key, string.Empty);
        }

        public static string GetString(this JToken self, string key, string defValue)
        {
            return self.Value<string>(key) ?? defValue;
        }

        public static bool GetBool(this JToken self, string key)
        {
            return self.Value<bool>(key);
        }

        public static bool GetBool(this JToken self, string key, bool defValue)
        {
            if (self[key] is null)
            {
                return defValue;
            }

            return self.Value<bool>(key);
        }

        public static bool GetBool(this JToken self, string key, out bool result)
        {
            if (self[key] == null)
            {
                result = default;
                return false;
            }

            result = self.Value<bool>(key);
            return true;
        }

        public static byte GetByte(this JToken self, string key)
        {
            return self.Value<byte>(key);
        }

        public static byte GetByte(this JToken self, string key, byte defValue)
        {
            if (self[key] == null)
            {
                return defValue;
            }

            return self.Value<byte>(key);
        }

        public static bool GetByte(this JToken self, string key, out byte result)
        {
            if (self[key] == null)
            {
                result = default;
                return false;
            }

            result = self.Value<byte>(key);
            return true;
        }

        public static sbyte GetInt8(this JToken self, string key)
        {
            return self.Value<sbyte>(key);
        }

        public static sbyte GetInt8(this JToken self, string key, sbyte defValue)
        {
            if (self[key] == null)
            {
                return defValue;
            }

            return self.Value<sbyte>(key);
        }

        public static bool GetInt8(this JToken self, string key, out sbyte result)
        {
            if (self[key] == null)
            {
                result = default;
                return false;
            }

            result = self.Value<sbyte>(key);
            return true;
        }

        public static short GetInt16(this JToken self, string key)
        {
            return self.Value<short>(key);
        }

        public static short GetInt16(this JToken self, string key, short defValue)
        {
            if (self[key] == null)
            {
                return defValue;
            }

            return self.Value<short>(key);
        }

        public static bool GetInt16(this JToken self, string key, out short result)
        {
            if (self[key] == null)
            {
                result = default;
                return false;
            }

            result = self.Value<short>(key);
            return true;
        }

        public static int GetInt32(this JToken self, string key, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (self[key] is null)
            {
                ErrorContainer.Add($"[JTokenExt] invalid key:{key}", file, line);
            }

            return self.Value<int>(key);
        }

        public static int GetInt32(this JToken self, string key, int defValue)
        {
            var subToken = self[key];
            if (subToken is null || string.IsNullOrEmpty(subToken.Value<string>()))
            {
                return defValue;
            }
            
            return subToken.Value<int>();
        }

        public static bool GetInt32(this JToken self, string key, out int result)
        {
            if (self[key] == null)
            {
                result = default;
                return false;
            }

            result = self.Value<int>(key);
            return true;
        }

        public static long GetInt64(this JToken self, string key)
        {
            return self.Value<long>(key);
        }

        public static long GetInt64(this JToken self, string key, long defValue)
        {
            if (self[key] == null)
            {
                return defValue;
            }

            return self.Value<long>(key);
        }

        public static bool GetInt64(this JToken self, string key, out long result)
        {
            if (self[key] == null)
            {
                result = default;
                return false;
            }

            result = self.Value<long>(key);
            return true;
        }

        public static float GetFloat(this JToken self, string key)
        {
            return self.Value<float>(key);
        }

        public static float GetFloat(this JToken self, string key, float defValue)
        {
            if (self[key] == null)
            {
                return defValue;
            }

            return self.Value<float>(key);
        }

        public static bool GetFloat(this JToken self, string key, out float result)
        {
            if (self[key] == null)
            {
                result = default;
                return false;
            }

            result = self.Value<float>(key);
            return true;
        }

        public static DateTime GetDateTime(this JToken self, string key)
        {
            var strValue = self.Value<string>(key) ?? throw new Exception($"json key not exist:{key}");
            return DateTime.Parse(strValue);
        }

        public static DateTime GetDateTime(this JToken self, string key, DateTime defValue)
        {
            var strValue = self.Value<string>(key);
            if (DateTime.TryParse(strValue, out var result) == false)
            {
                return defValue;
            }

            return result;
        }

        public static T GetEnum<T>(this JToken self, string key, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : struct, Enum
        {
            var strValue = GetString(self, key);
            if (string.IsNullOrEmpty(strValue))
            {
                ErrorContainer.Add($"[JTokenExt] invalid data key:{key} type:{typeof(T).Name}", file, line);
                return default;
            }

            if (Enum.TryParse<T>(strValue, ignoreCase: true, out var result) == false)
            {
                ErrorContainer.Add($"[JTokenExt] invalid enum value:{strValue} type:{typeof(T).Name}", file, line);
                return default;
            }

            return result;
        }

        public static T GetEnum<T>(this JToken self, string key, T defValue, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : struct, Enum
        {
            var strValue = GetString(self, key);
            if (string.IsNullOrEmpty(strValue))
            {
                return defValue;
            }

            if (Enum.TryParse<T>(strValue, ignoreCase: true, out var result) == false)
            {
                ErrorContainer.Add($"[JTokenExt] invalid enum value:{strValue} type:{typeof(T).Name}", file, line);
                return defValue;
            }

            return result;
        }

        public static bool TryGetEnum<T>(this JToken self, string key, out T result) where T : struct, Enum
        {
            var strValue = GetString(self, key, defValue: String.Empty);
            return Enum.TryParse<T>(strValue, ignoreCase: true, out result);
        }

        public static bool GetArray<T>(this JToken self, string key, in List<T> result, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (TryGetArray<T>(self, key, result, file, line) == false)
            {
                ErrorContainer.Add($"[JTokenExt] get array failed. key:{key} type:{typeof(T).Name}", file, line);
                return false;
            }

            return true;
        }

        public static bool GetArray<T>(this JToken self, string key, in List<T> result, Func<JToken, int, T> factory, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (TryGetArray<T>(self, key, result, factory) == false)
            {
                ErrorContainer.Add($"[JTokenExt] get array failed. key:{key} type:{typeof(T).Name}", file, line);
                return false;
            }

            return true;
        }

        public static bool TryGetArray<T>(this JToken self, string key, in List<T> result, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            var array = self[key] as JArray;
            if (array == null)
            {
                return false;
            }

            foreach (var data in array)
            {
                if (typeof(T).IsEnum)
                {
                    var strValue = data.ToString();
                    if (Enum.TryParse(typeof(T), strValue, ignoreCase: true, out var enumType) == false)
                    {
                        ErrorContainer.Add($"[JTokenExt] invalid enum value:{strValue} type:{typeof(T).Name}", file, line);
                        continue;
                    }

                    if (enumType != null)
                    {
                        result.Add((T)enumType);
                    }
                }
                else
                {
                    var value = data.Value<T>();
                    if (value != null)
                    {
                        result.Add(value);
                    }
                }
            }

            return true;
        }

        public static bool TryGetArray<T>(this JToken self, string key, in List<T> result, Func<JToken, int, T> factory)
        {
            var array = self[key] as JArray;
            if (array == null)
            {
                return false;
            }

            int index = 0;
            foreach (var data in array)
            {
                var value = factory.Invoke(data, index);
                if (value != null)
                {
                    result.Add(value);
                }

                ++index;
            }

            return true;
        }

        public static bool TryGetString(this JToken self, string key, out string result)
        {
            var targetToken = self[key];
            if (targetToken is null)
            {
                result = string.Empty;
                return false;
            }

            var targetValue = targetToken.Value<string>();
            if (targetValue is null)
            {
                result = string.Empty;
                return false;
            }

            result = targetValue;
            return true;
        }

        public static bool TryGetInt32(this JToken self, string key, out int result)
        {
            var targetToken = self[key];
            if (targetToken is null)
            {
                result = 0;
                return false;
            }

            result = targetToken.Value<int>();
            return true;
        }

        public static bool TryGetChild(this JToken self, string key, [MaybeNullWhen(false)] out JToken child)
        {
            if (self is null)
            {
                child = null;
                return false;
            }

            child = self[key];
            return child != null;
        }
    }
}
