namespace CutEditor.Model;

using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Core.Util;
using CutEditor.Model.Detail;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;
using static Du.Core.Messages;
using static Shared.Templet.Enums;

public sealed class ChoiceOption : ObservableObject
{
    private readonly L10nText text;
    private StartAnchorType jumpAnchor;

    public ChoiceOption(string defaultText)
    {
        this.text = new L10nText(defaultText);
        this.text.PropertyChanged += this.Text_PropertyChanged;
    }

    public ChoiceOption()
    {
        this.text = new L10nText();
        this.text.PropertyChanged += this.Text_PropertyChanged;
    }

    public long CutUid { get; private set; }
    public long ChoiceUid { get; private set; }
    public L10nText Text => this.text;
    public string UidString => $"{this.CutUid}-{this.ChoiceUid}";
    public StartAnchorType JumpAnchor
    {
        get => this.jumpAnchor;
        set => this.SetProperty(ref this.jumpAnchor, value);
    }

    public void InitializeUid(long cutUid, long choiceUid)
    {
        this.CutUid = cutUid;
        this.ChoiceUid = choiceUid;
    }

    //// -----------------------------------------------------------------------------------------

    internal static ChoiceOption? Load(JToken token, int index)
    {
        var result = new ChoiceOption();
        result.ChoiceUid = token.GetInt64("Uid", 0);
        result.text.Load(token, "JumpAnchorStringId");
        result.jumpAnchor = token.GetEnum("JumpAnchorId", StartAnchorType.None);

        return result;
    }

    internal ChoiceOutputFormat ToOutputType()
    {
        var result = new ChoiceOutputFormat
        {
            Uid = this.ChoiceUid,
            JumpAnchorStringId_KOR = this.text.AsNullable(L10nType.Korean),
            JumpAnchorStringId_ENG = this.text.AsNullable(L10nType.English),
            JumpAnchorStringId_JPN = this.text.AsNullable(L10nType.Japanese),
            JumpAnchorStringId_CHN = this.text.AsNullable(L10nType.ChineseSimplified),
        };

        if (this.JumpAnchor != StartAnchorType.None)
        {
            result.JumpAnchorId = this.JumpAnchor;
        }

        return result;
    }

    internal string GetSummaryText(L10nType l10nType)
    {
        var text = this.text.Get(l10nType);
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.Append($"{this.UidString} : {this.text.Get(l10nType)}");
        if (this.jumpAnchor != StartAnchorType.None)
        {
            sb.Append($" -> {this.jumpAnchor}");
        }

        return sb.ToString();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new DataChangedMessage());
    }

    private void Text_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new DataChangedMessage());
    }
}
