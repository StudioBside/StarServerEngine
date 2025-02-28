namespace Shared.Templet.Strings;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using StringStorage.SystemStrings;
using StringStorage.Translation;
using static System.Runtime.InteropServices.JavaScript.JSType;

public sealed class StringTable
{
    private readonly Dictionary<string, StringElement> uniqueElements = new();
    private readonly Dictionary<string, StringElement> allKeysElements = new();
    private readonly Dictionary<string, StringCategory> categories = new();

    public static StringTable Instance => Singleton<StringTable>.Instance;
    public IEnumerable<StringElement> Elements => this.uniqueElements.Values;
    public IEnumerable<string> CategoryNames => this.categories.Keys;
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

    public bool TryGetCategory(string name, [MaybeNullWhen(false)] out StringCategory category)
    {
        return this.categories.TryGetValue(name, out category);
    }

    internal void Load(string texTempletPath, SystemStringReader stringReader)
    {
        var l10nRoot = Path.Combine(texTempletPath, "L10N");
        foreach (var filePath in Directory.EnumerateFiles(l10nRoot, "*.exported", SearchOption.TopDirectoryOnly))
        {
            switch (Path.GetFileName(filePath))
            {
                case var f when f.StartsWith("STRING_"):
                    this.LoadValueString(filePath, stringReader);
                    break;
                case var f when f.StartsWith("KEY_STRING_"):
                    //this.LoadKeyString(filePath, stringReader);
                    break;

                default:
                    Log.Warn($"[StringTable] unknown file. filePath:{filePath}");
                    break;
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

    private void LoadValueString(string filePath, SystemStringReader stringReader)
    {
        var json = JsonUtil.Load(filePath);
        if (json["Data"] is not JArray jArray)
        {
            Log.ErrorAndExit($"[StringTable] file loading failed. fileName:{filePath}");
            return;
        }

        var categoryName = Path.GetFileNameWithoutExtension(filePath).Split('_')[^1];
        if (string.IsNullOrEmpty(categoryName))
        {
            Log.ErrorAndExit($"[StringTable] categoryName is empty. fileName:{filePath}");
            return;
        }

        var category = new StringCategory(categoryName);
        this.categories.Add(categoryName, category);

        if (stringReader.TryGetDb(categoryName, out var l10nDb) == false)
        {
            Log.Warn($"[StringTable] l10nDb 를 찾을 수 없습니다. categoryName:{categoryName}");
        }

        foreach (var token in jArray)
        {
            var element = new StringElement(token, categoryName, l10nDb);
            if (element == null)
            {
                continue;
            }

            if (this.uniqueElements.ContainsKey(element.PrimeKey))
            {
                ErrorContainer.Add($"[StringTable] duplicated key. key:{element.PrimeKey}");
            }
            else
            {
                this.uniqueElements.Add(element.PrimeKey, element);
            }

            foreach (var key in element.Keys)
            {
                if (this.allKeysElements.ContainsKey(key))
                {
                    ErrorContainer.Add($"[StringTable] duplicated key. key:{key}");
                }
                else
                {
                    this.allKeysElements.Add(key, element);
                }
            }

            category.Add(element);
        }
    }

    private void LoadKeyString(string filePath, SystemStringReader stringReader)
    {
        var json = JsonUtil.Load(filePath);
        if (json["Data"] is not JArray jArray)
        {
            Log.ErrorAndExit($"[StringTable] file loading failed. fileName:{filePath}");
            return;
        }

        var baseCategoryName = Path.GetFileNameWithoutExtension(filePath).Split('_')[^1];
        if (string.IsNullOrEmpty(baseCategoryName))
        {
            Log.ErrorAndExit($"[StringTable] categoryName is empty. fileName:{filePath}");
            return;
        }

        foreach (var token in jArray)
        {
            var categoryName = token.TryGetString("SubCategory", out var subCategoryName)
                ? subCategoryName : baseCategoryName;

            L10nReadOnlyDb? l10nDb = null;
            if (this.categories.TryGetValue(categoryName, out var category) == false)
            {
                category = new StringCategory(categoryName);
                this.categories.Add(categoryName, category);

                if (stringReader.TryGetDb(baseCategoryName, out l10nDb) == false)
                {
                    Log.Warn($"[StringTable] l10nDb 를 찾을 수 없습니다. categoryName:{baseCategoryName}");
                }
            }

            var element = new StringElement(token, baseCategoryName, l10nDb);
            if (element == null)
            {
                continue;
            }

            if (this.uniqueElements.ContainsKey(element.PrimeKey))
            {
                ErrorContainer.Add($"[StringTable] duplicated key. key:{element.PrimeKey}");
            }
            else
            {
                this.uniqueElements.Add(element.PrimeKey, element);
            }

            foreach (var key in element.Keys)
            {
                if (this.allKeysElements.ContainsKey(key))
                {
                    ErrorContainer.Add($"[StringTable] duplicated key. key:{key}");
                }
                else
                {
                    this.allKeysElements.Add(key, element);
                }
            }

            category.Add(element);
        }
    }
}
