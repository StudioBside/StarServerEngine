namespace StringStorage.Translation;

using System;
using System.Diagnostics.CodeAnalysis;
using Cs.Core.Util;
using StringStorage.Detail;

public sealed class L10nReadOnlyDb(string path, bool verbose) : IDisposable
{
    public const string ValueStringDbName = "ValueString.db";
    public const string KeyStringDbName = "KeyString.db";

    private readonly LevelDbReadOnly dbController = new(path, verbose);

    public void Dispose()
    {
        this.dbController.Dispose();
    }

    public SingleTextSet Get(string key)
    {
        if (this.dbController.TryGet(key, SingleTextSet.TryCreate, out var textSet) == false)
        {
            textSet = new SingleTextSet
            {
                CreatedAt = ServiceTime.Recent,
            };
        }

        return textSet;
    }

    public IEnumerable<string> GetKeys()
    {
        return this.dbController.Select(e => e.Key);
    }

    public bool TryGet(string key, [MaybeNullWhen(false)] out SingleTextSet textSet)
    {
        return this.dbController.TryGet(key, SingleTextSet.TryCreate, out textSet);
    }
}
