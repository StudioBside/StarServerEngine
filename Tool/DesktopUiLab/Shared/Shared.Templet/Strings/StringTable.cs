namespace Shared.Templet.Strings;

using System;
using System.Diagnostics.CodeAnalysis;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;

public sealed class StringTable
{
    private readonly Dictionary<string, StringElement> uniqueElements = new();
    private readonly Dictionary<string, StringElement> allKeysElements = new();

    public static StringTable Instance => Singleton<StringTable>.Instance;
    public IEnumerable<StringElement> Elements => this.uniqueElements.Values;

    public string Find(string key)
    {
        this.allKeysElements.TryGetValue(key, out var element);
        return string.IsNullOrEmpty(element?.Korean) ? key : element.Korean;
    }

    public StringElement? FindElement(string key)
    {
        this.allKeysElements.TryGetValue(key, out var element);
        return element;
    }

    public bool TryGetElement(string key, [MaybeNullWhen(false)] out StringElement element)
    {
        return this.allKeysElements.TryGetValue(key, out element);
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

        // 게임 내에 약속된 키워드들을 테이블에 함께 추가
        var localRules = new StringElement[]
        {
            new StringElement("PLAYER_NAME", "플레이 계정 닉네임"),
        };

        foreach (var local in localRules)
        {
            this.uniqueElements.Add(local.PrimeKey, local);
            this.allKeysElements.Add(local.PrimeKey, local);
        }

        Log.Info($"[StringTable] loaded. uniqueCount:{this.uniqueElements.Count} allKeyCount:{this.allKeysElements.Count}");
    }
}
