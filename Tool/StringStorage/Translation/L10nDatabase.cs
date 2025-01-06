namespace StringStorage.Translation;

using System;
using Cs.Core.Perforce;
using Cs.Core.Util;
using StringStorage.Detail;

public sealed class L10nDatabase : IDisposable
{
    private readonly LevelDbController dbController;

    public L10nDatabase(string path, P4Commander p4Commander)
    {
        this.dbController = new LevelDbController(path, p4Commander);
    }

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

    public void Upsert(string key, SingleTextSet textSet)
    {
        textSet.UpdatedAt = ServiceTime.Recent;
        this.dbController.Upsert(key, textSet.ToString());
    }
}
