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
    private readonly Dictionary<string, StringElementSet> elementSets = new();

    public static StringTable Instance => Singleton<StringTable>.Instance;
    public IEnumerable<StringElement> Elements => this.uniqueElements.Values;
    public IEnumerable<string> CategoryNames => this.elementSets.Keys;
    public int UniqueCount => this.uniqueElements.Count;

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
        var l10nRoot = Path.Combine(fullPath, "L10N");
        foreach (var file in Directory.EnumerateFiles(l10nRoot, "*.exported", SearchOption.TopDirectoryOnly))
        {
            var json = JsonUtil.Load(file);
            if (json["Data"] is not JArray jArray)
            {
                Log.ErrorAndExit($"[StringTable] file loading failed. fileName:{file}");
                return;
            }

            var categoryName = Path.GetFileNameWithoutExtension(file).Split('_')[^1];
            if (string.IsNullOrEmpty(categoryName))
            {
                Log.ErrorAndExit($"[StringTable] categoryName is empty. fileName:{file}");
                return;
            }

            var set = new StringElementSet(categoryName);
            this.elementSets.Add(categoryName, set);

            foreach (var token in jArray)
            {
                var element = new StringElement(token, categoryName);
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

                set.Add(element);
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
