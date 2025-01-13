namespace CutEditor.ViewModel.L10n;

using System;
using System.Runtime.CompilerServices;
using Cs.Core.Util;
using CutEditor.Model.L10n;
using static CutEditor.Model.Enums;
using static StringStorage.Enums;

public abstract class L10nStrategyBase(L10nSourceType sourceType) : IL10nStrategy
{
    private readonly int[] statistics = new int[EnumUtil<L10nMappingState>.Count];

    public L10nSourceType SourceType { get; } = sourceType;
    public abstract IReadOnlyList<IL10nMapping> Mappings { get; }
    public IReadOnlyList<int> Statistics => this.statistics;

    public abstract bool ImportFile(string fileFullPath, ISet<string> importedHeaders);
    public abstract bool LoadOriginData(string name);
    public abstract bool SaveToFile(string name, L10nType l10nType);

    //// ------------------------------------------------------------------------

    protected void ClearStatistics()
    {
        Array.Clear(this.statistics);
    }

    protected void IncreaseStatistics(L10nMappingState state)
    {
        ++this.statistics[(int)state];
    }
}
