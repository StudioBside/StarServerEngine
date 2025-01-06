namespace StringStorage.SystemStrings;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cs.Core;
using Cs.Logging;
using StringStorage.Translation;

public sealed class SystemStringReader : IDisposable
{
    private readonly Dictionary<string /*category*/, L10nReadOnlyDb> dbList = new();
    private string rootPath = string.Empty;
    public static SystemStringReader Instance => Singleton<SystemStringReader>.Instance;
    public bool Initialize(string dbRoot)
    {
        this.rootPath = dbRoot;

        // root에 존재하는 zip 파일을 대상으로 초기 로딩
        var zipFiles = Directory.GetFiles(this.rootPath, "*.zip");
        foreach (var zipFile in zipFiles)
        {
            var category = Path.GetFileNameWithoutExtension(zipFile);
            var db = new L10nReadOnlyDb(Path.Combine(this.rootPath, category));
            this.dbList.Add(category, db);
        }

        Log.Debug($"SystemStringReader.Initialize. rootPath:{this.rootPath} dbList:{this.dbList.Count}");
        return true;
    }

    public void Dispose()
    {
        foreach (var data in this.dbList.Values)
        {
            data.Dispose();
        }
    }

    public bool TryGetDb(string category, [MaybeNullWhen(false)] out L10nReadOnlyDb db)
    {
        return this.dbList.TryGetValue(category, out db);
    }

    public SingleTextSet GetTextSet(string category, string key)
    {
        if (this.dbList.TryGetValue(category, out var db) == false)
        {
            Log.Error($"string category not found. category:{category} key:{key}");
            return SingleTextSet.Empty;
        }

        return db.Get(key);
    }
}
