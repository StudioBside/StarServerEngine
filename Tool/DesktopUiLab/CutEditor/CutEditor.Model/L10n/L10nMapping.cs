namespace CutEditor.Model.L10n;

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CutEditor.Model.Detail;
using static CutEditor.Model.Enums;

public sealed class L10nMapping : ObservableObject
{
    private readonly Cut cut;
    private readonly ChoiceOption? choice;
    private readonly L10nText l10nText;
    private CutOutputExcelFormat? imported;
    private L10nMappingType mappingType;

    public L10nMapping(Cut cut)
    {
        this.UidStr = cut.Uid.ToString();
        this.cut = cut;
        this.l10nText = cut.UnitTalk;
    }

    public L10nMapping(Cut cut, ChoiceOption choice)
    {
        this.UidStr = choice.UidString;
        this.cut = cut;
        this.choice = choice;
        this.l10nText = choice.Text;
    }

    public string UidStr { get; }
    public L10nText L10NText => this.l10nText;
    public CutOutputExcelFormat? Imported
    {
        get => this.imported;
        set => this.SetProperty(ref this.imported, value);
    }

    public L10nMappingType MappingType
    {
        get => this.mappingType;
        set => this.SetProperty(ref this.mappingType, value);
    }

    //// -----------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.Imported):
                this.CalcMappingType();
                break;
        }
    }

    private void CalcMappingType()
    {
        if (this.imported == null)
        {
            this.MappingType = L10nMappingType.MissingImported;
        }
        else if (this.imported.Korean != this.l10nText.Korean)
        {
            this.MappingType = L10nMappingType.TextChanged;
        }
        else
        {
            this.MappingType = L10nMappingType.Normal;
        }
    }
}
