namespace CutEditor.Model.L10n;

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CutEditor.Model;
using CutEditor.Model.Detail;
using static CutEditor.Model.Enums;
using static StringStorage.Enums;

public sealed class L10nMappingNormal : ObservableObject, IL10nMapping
{
    private readonly Cut cut;
    private readonly L10nText l10nText;
    private CutOutputExcelFormat? imported;
    private L10nMappingState mappingState;

    public L10nMappingNormal(Cut cut)
    {
        this.UidStr = cut.Uid.ToString();
        this.cut = cut;
        this.l10nText = cut.UnitTalk;
    }

    public L10nMappingNormal(Cut cut, ChoiceOption choice)
    {
        this.UidStr = choice.UidString;
        this.cut = cut;
        this.l10nText = choice.Text;
    }

    public string UidStr { get; }
    public L10nText L10nText => this.l10nText;
    public Cut Cut => this.cut;
    public CutOutputExcelFormat? Imported
    {
        get => this.imported;
        set => this.SetProperty(ref this.imported, value);
    }

    public L10nMappingState MappingState
    {
        get => this.mappingState;
        set => this.SetProperty(ref this.mappingState, value);
    }

    public bool ApplyData(L10nType l10nType)
    {
        if (this.imported == null)
        {
            return false;
        }

        var newValue = this.imported.Get(l10nType);
        var oldValue = this.l10nText.Set(l10nType, newValue);

        return oldValue != newValue;
    }

    //// -----------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.Imported):
                this.CalcMappingState();
                break;
        }
    }

    private void CalcMappingState()
    {
        if (this.imported == null)
        {
            this.mappingState = L10nMappingState.MissingImported;
        }
        else if (this.imported.Korean != this.l10nText.Korean)
        {
            this.mappingState = L10nMappingState.TextChanged;
        }
        else
        {
            this.mappingState = L10nMappingState.Normal;
        }
    }
}
