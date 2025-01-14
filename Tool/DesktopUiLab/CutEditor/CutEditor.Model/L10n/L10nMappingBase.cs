namespace CutEditor.Model.L10n;

using System;
using CutEditor.Model.Interfaces;
using static CutEditor.Model.Enums;

public abstract class L10nMappingBase
{
    public string UidStr { get; protected set; } = string.Empty;
    public abstract IL10nText SourceData { get; }
    public abstract IL10nText? ImportedData { get; }
    public L10nMappingState MappingState { get; private set; }

    protected void CalcMappingState()
    {
        if (this.ImportedData == null)
        {
            this.MappingState = L10nMappingState.MissingImported;
        }
        else if (this.ImportedData.Korean != this.SourceData.Korean)
        {
            this.MappingState = L10nMappingState.TextChanged;
        }
        else
        {
            this.MappingState = L10nMappingState.Normal;
        }
    }
}
