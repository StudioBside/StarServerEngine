namespace StringStorage.Translation;

using System;
using Cs.Core.Util;
using StringStorage.Detail;

public sealed class L10nReadOnlyDb(string path) : IDisposable
{
    private readonly LevelDbReadOnly dbController = new(path);

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
}
