namespace Du.Core.Util;

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Cs.Logging;
using Newtonsoft.Json.Linq;

public static class JsonElementExt
{
    public static bool GetBoolean(this JsonElement self, string propertyName)
    {
        return self.GetProperty(propertyName).GetBoolean();
    }

    public static bool GetBoolean(this JsonElement self, string propertyName, bool defaultValue)
    {
        return self.TryGetProperty(propertyName, out var property) ? property.GetBoolean() : defaultValue;
    }

    public static int GetInt32(this JsonElement self, string propertyName)
    {
        return self.GetProperty(propertyName).GetInt32();
    }

    public static int GetInt32(this JsonElement self, string propertyName, int defaultValue)
    {
        return self.TryGetProperty(propertyName, out var property) ? property.GetInt32() : defaultValue;
    }

    public static string GetString(this JsonElement self, string propertyName)
    {
        return GetString(self, propertyName, string.Empty);
    }

    public static string GetString(this JsonElement self, string propertyName, string defaultValue)
    {
        if (self.TryGetProperty(propertyName, out var property) == false)
        {
            return defaultValue;
        }

        return property.GetString() ?? defaultValue;
    }

    public static T GetEnum<T>(this JsonElement self, string key, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : struct, Enum
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

    public static T GetEnum<T>(this JsonElement self, string key, T defValue, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where T : struct, Enum
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

    public static bool TryGetEnum<T>(this JsonElement self, string key, out T result) where T : struct, Enum
    {
        var strValue = GetString(self, key, defaultValue: string.Empty);
        return Enum.TryParse<T>(strValue, ignoreCase: true, out result);
    }
    
    public static bool TryGetArray(this JsonElement self, string propertyName, in List<int> result, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (self.TryGetProperty(propertyName, out var arrayElement) == false)
        {
            return false;
        }

        foreach (var element in arrayElement.EnumerateArray())
        {
            var value = element.GetInt32();
            result.Add(value);
        }

        return true;
    }

    public static bool GetArray(this JsonElement self, string propertyName, in List<string> result, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (TryGetArray(self, propertyName, result) == false)
        {
            Log.Error($"[JsonElementExt] get array failed. propretyName:{propertyName} type:string");
            return false;
        }

        return true;
    }

    public static bool TryGetArray(this JsonElement self, string propertyName, in List<string> result, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (self.TryGetProperty(propertyName, out var arrayElement) == false)
        {
            return false;
        }

        foreach (var element in arrayElement.EnumerateArray())
        {
            var value = element.GetString(string.Empty);
            result.Add(value);
        }

        return true;
    }

    public static bool GetArray<T>(this JsonElement self, string propertyName, in List<T> result, Func<JsonElement, T> converter, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (TryGetArray(self, propertyName, result, converter) == false)
        {
            Log.Error($"[JsonElementExt] get array failed. propretyName:{propertyName} type:{typeof(T).Name}");
            return false;
        }

        return true;
    }

    public static bool TryGetArray<T>(this JsonElement self, string propertyName, in List<T> result, Func<JsonElement, T> converter, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (self.TryGetProperty(propertyName, out var arrayElement) == false)
        {
            return false;
        }

        foreach (var element in arrayElement.EnumerateArray())
        {
            var value = converter.Invoke(element);
            result.Add(value);
        }

        return true;
    }
}
