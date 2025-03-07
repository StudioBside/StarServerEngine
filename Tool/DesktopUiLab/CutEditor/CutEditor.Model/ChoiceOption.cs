namespace CutEditor.Model;

using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Core.Util;
using CutEditor.Model.Detail;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;
using static CutEditor.Model.Messages;
using static Du.Core.Messages;
using static StringStorage.Enums;

public sealed class ChoiceOption : ObservableObject
{
    private readonly L10nText text;
    private StartAnchorType jumpAnchor;

    public ChoiceOption(long cutUid, long choiceUid, string defaultText)
    {
        this.CutUid = cutUid;
        this.ChoiceUid = choiceUid;
        this.text = new L10nText(defaultText);
        this.text.PropertyChanged += this.Text_PropertyChanged;
    }

    public ChoiceOption(long cutUid, long choiceUid)
    {
        this.CutUid = cutUid;
        this.ChoiceUid = choiceUid;
        this.text = new L10nText();
        this.text.PropertyChanged += this.Text_PropertyChanged;
    }

    public long CutUid { get; }
    public long ChoiceUid { get; }
    public L10nText Text => this.text;
    public string UidString => $"{this.CutUid}-{this.ChoiceUid}";
    public StartAnchorType JumpAnchor
    {
        get => this.jumpAnchor;
        set => this.SetProperty(ref this.jumpAnchor, value);
    }

    //// -----------------------------------------------------------------------------------------

    internal static ChoiceOption? Load(JToken token, long cutUid)
    {
        var result = new ChoiceOption(cutUid, token.GetInt64("Uid"));

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
            JumpAnchorStringId_CHNS = this.text.AsNullable(L10nType.ChineseSimplified),
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
        WeakReferenceMessenger.Default.Send(new UpdateChoicePreviewMessage());
    }

    private void Text_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new DataChangedMessage());
    }
}
