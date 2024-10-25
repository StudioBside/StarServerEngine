namespace CutEditor.Model;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;
using static CutEditor.Model.NKMCutsceneEnums;

public sealed class ChoiceOption : ObservableObject
{
    private readonly L10nText text = new();
    private JumpAnchorType jumpAnchor = JumpAnchorType.None;
    private RewardAnchorType rewardAnchor = RewardAnchorType.None;

    public long CutUid { get; private set; }
    public long ChoiceUid { get; private set; }
    public L10nText Text => this.text;
    public string? JumpAnchorId { get; private set; }
    public string UidString => $"{this.CutUid}-{this.ChoiceUid}";
    public JumpAnchorType JumpAnchor
    {
        get => this.jumpAnchor;
        set => this.SetProperty(ref this.jumpAnchor, value);
    }

    public RewardAnchorType RewardAnchor
    {
        get => this.rewardAnchor;
        set => this.SetProperty(ref this.rewardAnchor, value);
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
        result.text.Load(token, "JumpAnchorStringId");
        result.JumpAnchorId = token.GetString("JumpAnchorId", null!);
        if (string.IsNullOrEmpty(result.JumpAnchorId) == false)
        {
            if (Enum.TryParse<JumpAnchorType>(result.JumpAnchorId, out var jumpAnchor))
            {
                result.JumpAnchor = jumpAnchor;
            }
            else if (Enum.TryParse<RewardAnchorType>(result.JumpAnchorId, out var rewardAnchor))
            {
                result.RewardAnchor = rewardAnchor;
            }
            else
            {
                ErrorContainer.Add($"[BranchData] JumpAnchorId 파싱에 실패했습니다. idString:{result.JumpAnchorId}");
            }
        }

        return result;
    }

    internal object ToOutputType()
    {
        return new
        {
            JumpAnchorStringId_KOR = this.text.AsNullable(L10nType.Korean),
            JumpAnchorStringId_ENG = this.text.AsNullable(L10nType.English),
            JumpAnchorStringId_JPN = this.text.AsNullable(L10nType.Japanese),
            JumpAnchorStringId_CHN = this.text.AsNullable(L10nType.ChineseSimplified),
            JumpAnchorId = this.JumpAnchorId,
        };
    }
}
