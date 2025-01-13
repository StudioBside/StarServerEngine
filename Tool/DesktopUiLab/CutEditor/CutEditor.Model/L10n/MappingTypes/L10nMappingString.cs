namespace CutEditor.Model.L10n.MappingTypes;

using CutEditor.Model.ExcelFormats;
using Shared.Interfaces;
using Shared.Templet.Strings;
using static StringStorage.Enums;

public sealed class L10nMappingString : L10nMappingBase, IL10nMapping
{
    private readonly StringElement sourceData;
    private StringOutputExcelFormat? importedData;

    public L10nMappingString(string primeKey, StringElement stringElement)
    {
        this.UidStr = primeKey;
        this.sourceData = stringElement;
    }

    public override IL10nText SourceData => this.sourceData;
    public override IL10nText? ImportedData => this.importedData;

    public bool ApplyData(L10nType l10nType)
    {
        if (this.importedData == null)
        {
            return false;
        }

        var newValue = this.importedData.Get(l10nType);
        var oldValue = this.sourceData.Set(l10nType, newValue);
        return oldValue != newValue;
    }

    public void SetImported(StringOutputExcelFormat? imported)
    {
        this.importedData = imported;
        this.CalcMappingState();
    }
}
