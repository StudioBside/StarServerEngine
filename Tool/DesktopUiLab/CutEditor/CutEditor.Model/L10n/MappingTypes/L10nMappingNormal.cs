namespace CutEditor.Model.L10n;

using CutEditor.Model;
using CutEditor.Model.ExcelFormats;
using Shared.Interfaces;
using static StringStorage.Enums;

public sealed class L10nMappingNormal : L10nMappingBase, IL10nMapping
{
    private readonly Cut cut;
    private readonly L10nText sourceData;
    private CutOutputExcelFormat? importedData;

    public L10nMappingNormal(Cut cut)
    {
        this.UidStr = cut.Uid.ToString();
        this.cut = cut;
        this.sourceData = cut.UnitTalk;
    }

    public L10nMappingNormal(Cut cut, ChoiceOption choice)
    {
        this.UidStr = choice.UidString;
        this.cut = cut;
        this.sourceData = choice.Text;
    }

    public override IL10nText SourceData => this.sourceData;
    public override IL10nText? ImportedData => this.importedData;
    public Cut Cut => this.cut;

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

    public void SetImported(CutOutputExcelFormat? imported)
    {
        this.importedData = imported;
        this.CalcMappingState();
    }

    //// -----------------------------------------------------------------------------------------
}
