namespace Shared.Templet.Strings;

using System;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;

public sealed class StringTable
{
    private readonly Dictionary<string, StringElement> elements = new();

    public static StringTable Instance => Singleton<StringTable>.Instance;

    public string Find(string key)
    {
        this.elements.TryGetValue(key, out var element);
        return string.IsNullOrEmpty(element?.Korean) ? key : element.Korean;
    }

    internal void Load(string fullPath)
    {
        var json = JsonUtil.Load(fullPath);
        if (json["Data"] is not JArray jArray)
        {
            Log.ErrorAndExit($"[StringTable] file loading failed. fileName:{fullPath}");
            return;
        }

        foreach (var token in jArray)
        {
            var element = new StringElement(token);
            if (element == null)
            {
                continue;
            }

            foreach (var key in element.Keys)
            {
                if (this.elements.ContainsKey(key))
                {
                    Log.ErrorAndExit($"[StringTable] duplicated key. key:{key}");
                }

                this.elements.Add(key, element);
            }
        }

        Log.Info($"[StringTable] loaded. count:{this.elements.Count}");
    }
}
