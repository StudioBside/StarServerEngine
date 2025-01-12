namespace Shared.Templet.Strings;

using System.Collections.Generic;
using Cs.Logging;

public sealed class StringCategory(string name)
{
    private readonly Dictionary<string, StringElement> uniqueElements = new();
    private readonly Dictionary<string, StringElement> allKeysElements = new();

    public string Name { get; } = name;
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