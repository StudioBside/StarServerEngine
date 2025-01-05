namespace Shared.Templet.Strings;

using System;
using System.Collections.Generic;
using Cs.Logging;

internal sealed class StringElementSet
{
    private readonly Dictionary<string, StringElement> uniqueElements = new();
    private readonly Dictionary<string, StringElement> allKeysElements = new();

    public StringElementSet(string name)
    {
        this.Name = name;
    }

    public string Name { get; }
    public IEnumerable<StringElement> Elements => this.uniqueElements.Values;
    public int UniqueCount => this.uniqueElements.Count;

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
}