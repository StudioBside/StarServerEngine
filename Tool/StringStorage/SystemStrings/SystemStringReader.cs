namespace StringStorage.SystemStrings;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cs.Core;
using Cs.Logging;
using StringStorage.Translation;

/// <summary>
/// root 경로에 존재하는 zip 파일을 모두 연결해두는 (=모든 카테고리를 읽는) 클래스.
/// </summary>
public sealed class SystemStringReader : IDisposable
{
    private readonly Dictionary<string /*dbName*/, L10nReadOnlyDb> dbList = new(StringComparer.OrdinalIgnoreCase);
    public bool Initialize(string dbRoot, bool verbose)
    {
        // root에 존재하는 zip 파일을 대상으로 초기 로딩
        var zipFiles = Directory.GetFiles(dbRoot, "*.zip");
        foreach (var zipFile in zipFiles)
        {
            var dbName = Path.GetFileNameWithoutExtension(zipFile);
            var db = new L10nReadOnlyDb(Path.Combine(dbRoot, dbName), verbose);
            this.dbList.Add(dbName, db);
        }

        if (verbose)
        {
            Log.Debug($"SystemStringReader.Initialize. rootPath:{dbRoot} dbList:{this.dbList.Count}");
        }

        return true;
    }

    public void Dispose()
    {
        foreach (var data in this.dbList.Values)
        {
            data.Dispose();
        }
    }

    public bool TryGetDb(string dbName, [MaybeNullWhen(false)] out L10nReadOnlyDb db)
    {
        return this.dbList.TryGetValue(dbName, out db);
    }

    public SingleTextSet GetTextSet(string dbName, string key)
    {
        if (this.dbList.TryGetValue(dbName, out var db) == false)
        {
            Log.Error($"string category not found. dbName:{dbName} key:{key}");
            return SingleTextSet.Empty;
        }

        return db.Get(key);
    }
}
