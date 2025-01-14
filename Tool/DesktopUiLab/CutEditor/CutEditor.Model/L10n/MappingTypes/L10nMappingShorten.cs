namespace CutEditor.Model.L10n;

using CutEditor.Model;
using CutEditor.Model.ExcelFormats;
using CutEditor.Model.Interfaces;
using static StringStorage.Enums;

public sealed class L10nMappingShorten : L10nMappingBase, IL10nMapping
{
    private readonly Cut cut;
    private readonly L10nText sourceData;
    private ShortenCutOutputExcelFormat? importedData;

    public L10nMappingShorten(string fileName, Cut cut)
    {
        this.FileName = fileName;
        this.UidStr = cut.Uid.ToString();
        this.cut = cut;
        this.sourceData = cut.UnitTalk;
    }

    public L10nMappingShorten(string fileName, Cut cut, ChoiceOption choice)
    {
        this.FileName = fileName;
        this.UidStr = choice.UidString;
        this.cut = cut;
        this.sourceData = choice.Text;
    }

    public string FileName { get; }
    public string FileAndUid => $"{this.FileName} ({this.UidStr})";
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

    public void SetImported(ShortenCutOutputExcelFormat? imported)
    {
        this.importedData = imported;
        this.CalcMappingState();
    }

    public override string ToString()
    {
        return this.FileAndUid;
    }

    //// -----------------------------------------------------------------------------------------
}
