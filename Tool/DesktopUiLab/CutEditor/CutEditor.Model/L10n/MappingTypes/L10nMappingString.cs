namespace CutEditor.Model.L10n.MappingTypes;

using CutEditor.Model.ExcelFormats;
using CutEditor.Model.Interfaces;
using Shared.Templet.Strings;
using static StringStorage.Enums;

public sealed class L10nMappingString : L10nMappingBase, IL10nMapping
{
    private readonly StringElement stringElement;
    private readonly L10nText sourceData; // 화면에 출력되는 데이터. 바인딩을 써햐해서 ObservableObject여야 한다.
    private StringOutputExcelFormat? importedData;

    public L10nMappingString(string primeKey, StringElement stringElement)
    {
        this.UidStr = primeKey;
        this.stringElement = stringElement;
        this.sourceData = new L10nText(stringElement);
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
        var oldValue = this.stringElement.Set(l10nType, newValue);

        bool updated = oldValue != newValue;
        if (updated)
        {
            this.sourceData.Set(l10nType, newValue);
        }

        return updated;
    }

    public void SetImported(StringOutputExcelFormat? imported)
    {
        this.importedData = imported;
        this.CalcMappingState();
    }
}
