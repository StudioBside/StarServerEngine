namespace CutEditor.Model;

using System;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;
using static NKM.NKMCutsceneEnums;

public sealed class ChoiceOption
{
    private readonly L10nText text = new();
   
    public L10nText Text => this.text;
    public string? JumpAnchorId { get; private set; }
    public JumpAnchorType JumpAnchor { get; private set; } = JumpAnchorType.None;
    public RewardAnchorType RewardAnchor { get; private set; } = RewardAnchorType.None;

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
