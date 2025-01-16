namespace JsonMigrator;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cs.Logging;

internal sealed class MigrationStepContioner : IOrderedEnumerable<MigrationStep>
{
    private readonly IOrderedEnumerable<MigrationStep> steps;

    private MigrationStepContioner(IReadOnlyDictionary<int, MigrationStep> steps)
    {
        this.steps = steps.Values.OrderBy(x => x.Version);
        this.Count = steps.Count;
    }

    public int Count { get; }

    public static MigrationStepContioner? Create(string stepScriptPath)
    {
        Dictionary<int, MigrationStep> loadSteps = new();
        if (Directory.Exists(stepScriptPath) == false)
        {
            Log.Info($"마이그레이션 스크립트 폴더가 없습니다. 성공으로 처리됩니다.");
            return new MigrationStepContioner(loadSteps);
        }

        foreach (var stepFile in Directory.EnumerateFiles(stepScriptPath))
        {
            var migration = MigrationStep.Create(stepFile);
            if (migration is null)
            {
                Log.Error($"invalid migration step:{stepFile}");
                return null;
            }

            if (loadSteps.TryGetValue(migration.Version, out var exist))
            {
                Log.Error($"duplicated migration version:{migration.Version} file1:{exist.FilePath} file2:{stepFile}");
                return null;
            }

            loadSteps.Add(migration.Version, migration);
        }

        return new MigrationStepContioner(loadSteps);
    }

    public IOrderedEnumerable<MigrationStep> CreateOrderedEnumerable<TKey>(Func<MigrationStep, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
    {
        var ordered = descending ? this.steps.OrderByDescending(keySelector, comparer) : this.steps.OrderBy(keySelector, comparer);
        return ordered;
    }

    public IEnumerator<MigrationStep> GetEnumerator()
    {
        return this.steps.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}