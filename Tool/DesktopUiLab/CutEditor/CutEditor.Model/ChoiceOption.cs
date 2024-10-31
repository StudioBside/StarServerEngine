namespace CutEditor.Model;

using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using CutEditor.Model.Detail;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;

public sealed class ChoiceOption : ObservableObject
{
    private readonly L10nText text = new();
    private StartAnchorType jumpAnchor;

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

    internal string GetSummaryText()
    {
        var sb = new StringBuilder();
        sb.Append($"{this.UidString} : {this.text.Korean}");
        if (this.jumpAnchor != StartAnchorType.None)
        {
            sb.Append($" -> {this.jumpAnchor}");
        }

        return sb.ToString();
    }
}
