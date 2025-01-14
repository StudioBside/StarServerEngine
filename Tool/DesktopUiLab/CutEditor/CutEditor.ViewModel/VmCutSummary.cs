namespace CutEditor.ViewModel;

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CutEditor.Model;
using CutEditor.ViewModel.Detail;
using static CutEditor.ViewModel.Enums;
using static StringStorage.Enums;

public sealed class VmCutSummary : ObservableObject
{
    private readonly ChoiceUidGenerator choiceUidGenerator;
    private CutDataType dataType;

    public VmCutSummary(Cut cut, string cutsceneName)
    {
        this.Cut = cut;
        cut.PropertyChanged += this.Cut_PropertyChanged;
        this.dataType = cut.Choices.Count > 0
            ? CutDataType.Branch
            : CutDataType.Normal;

        this.CutsceneName = cutsceneName;
        this.choiceUidGenerator = new(cut.Uid, cut.Choices);
    }

    public string CutsceneName { get; }
    public Cut Cut { get; }
   
    public string SummaryKorean => this.Cut.GetSummaryText(L10nType.Korean);
    public string SummaryEnglish => this.Cut.GetSummaryText(L10nType.English);
    public string SummaryJapanese => this.Cut.GetSummaryText(L10nType.Japanese);
    public string SummaryChineseSimplified => this.Cut.GetSummaryText(L10nType.ChineseSimplified);

    public CutDataType DataType
    {
        get => this.dataType;
        set => this.SetProperty(ref this.dataType, value);
    }

    public override string ToString()
    {
        return $"VmCutSummary. Uid:{this.Cut.Uid}";
    }

    //// --------------------------------------------------------------------------------------------

    private void Cut_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Model.Cut.EmotionEffect):
                break;
        }
    }
}