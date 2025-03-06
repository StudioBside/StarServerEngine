namespace Shared.Templet.Strings;

using System.Collections.Generic;
using Cs.Logging;

public enum SystemStringType
{
    ValueString,
    KeyString,
}

public sealed class StringCategory(string name, SystemStringType systemStringType)
{
    private readonly Dictionary<string, StringElement> uniqueElements = new();
    private readonly Dictionary<string, StringElement> allKeysElements = new();

    public string Name { get; } = name;
    public SystemStringType SystemStringType { get; } = systemStringType;
    public IEnumerable<StringElement> Elements => this.uniqueElements.Values;
    public int UniqueCount => this.uniqueElements.Count;

    public static bool GuessStringTypeByFileName(string fileName, out SystemStringType systemStringType)
    {
        if (fileName.StartsWith("VALUE_STRING_"))
        {
            systemStringType = SystemStringType.ValueString;
            return true;
        }

        if (fileName.StartsWith("KEY_STRING_"))
        {
            systemStringType = SystemStringType.KeyString;
            return true;
        }

        systemStringType = default;
        return false;
    }

    public void Add(StringElement element)
    {
        this.uniqueElements.Add(element.PrimeKey, element);
        foreach (var key in element.Keys)
        {
            if (this.allKeysElements.ContainsKey(key))
            {
                Log.ErrorAndExit($"[StringTable] duplicated key. key:{key}");
            }

            this.allKeysElements.Add(key, element);
        }
    }

    public string GetExportFileName()
    {
        return this.SystemStringType switch
        {
            SystemStringType.ValueString => $"VALUE_STRING_{this.Name}.xlsx",
            SystemStringType.KeyString => $"KEY_STRING_{this.Name}.xlsx",
            _ => string.Empty,
        };
    }
}