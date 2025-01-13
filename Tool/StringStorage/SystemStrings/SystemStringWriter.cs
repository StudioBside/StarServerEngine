namespace StringStorage.SystemStrings;

using System;
using System.Diagnostics.CodeAnalysis;
using Cs.Core.Perforce;
using Cs.Logging;
using StringStorage.Translation;
using static StringStorage.Enums;

/// <summary>
/// 시스템 스트링 카테고리 1개에 대한 스트링을 업데이트하는 클래스.
/// </summary>
public sealed class SystemStringWriter : IDisposable
{
    private readonly L10nDatabase db;

    private SystemStringWriter(L10nDatabase db)
    {
        this.db = db;
    }

    public static bool Create(string dbRoot, string categoryName, [MaybeNullWhen(false)] out SystemStringWriter writer)
    {
        writer = null;

        if (P4Commander.TryCreate(out var p4Commander) == false)
        {
            Log.Error($"p4 connection failed.");
            return false;
        }

        var dbPath = Path.Combine(dbRoot, categoryName);
        var db = new L10nDatabase(dbPath, p4Commander);
        writer = new SystemStringWriter(db);
        return true;
    }

    public void Dispose()
    {
        this.db.Dispose();
    }

    public void Upsert(string key, L10nType l10nType, string value)
    {
        var textSet = this.db.Get(key);
        textSet.SetValueString(l10nType, value);
        this.db.Upsert(key, textSet);
    }
}
