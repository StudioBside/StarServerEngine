namespace CutEditor.Model;

using System.ComponentModel;

public static class Enums
{
    public enum CutsceneType
    {
        NCT_MAIN,
        NCT_JOURNEY,
        NCT_UNIT,
        NCT_ARCANA,
        NCT_REST,
        NCT_ETC,
        NCT_TEST,
        NCT_REQUEST,
        NCT_TRAINING,
        NCT_PERSONAL,
        NCT_EVENT,
    }

    public enum L10nType
    {
        [Description("한국어")]
        Korean,
        [Description("영어")]
        English,
        [Description("일본어")]
        Japanese,
        [Description("중국어(간체)")]
        ChineseSimplified, // 간체
        [Description("중국어(번체)")]
        ChineseTraditional, // 번체
    }

    public enum StartAnchorType
    {
        None,
        JUMPANCHOR_1,
        JUMPANCHOR_2,
        JUMPANCHOR_3,
        JUMPANCHOR_4,
        JUMPANCHOR_5,
        REWARD_ANCHOR_1,
        REWARD_ANCHOR_2,
        REWARD_ANCHOR_3,
        REWARD_ANCHOR_4,
        REWARD_ANCHOR_5,
    }

    public enum DestAnchorType
    {
        None,
        JUMPANCHOR_1,
        JUMPANCHOR_1_SUCCESS,
        JUMPANCHOR_1_FAIL,
        JUMPANCHOR_2,
        JUMPANCHOR_2_SUCCESS,
        JUMPANCHOR_2_FAIL,
        JUMPANCHOR_3,
        JUMPANCHOR_3_SUCCESS,
        JUMPANCHOR_3_FAIL,
        JUMPANCHOR_4,
        JUMPANCHOR_4_SUCCESS,
        JUMPANCHOR_4_FAIL,
        JUMPANCHOR_5,
        JUMPANCHOR_5_SUCCESS,
        JUMPANCHOR_5_FAIL,
        JUMPANCHOR_FINISH,
        REWARD_ANCHOR_1,
    }

    public static string ToJsonKey(this L10nType self, string prefix)
    {
        var suffix = self switch
        {
            L10nType.Korean => "KOR",
            L10nType.English => "ENG",
            L10nType.Japanese => "JPN",
            L10nType.ChineseSimplified => "CHN", // "CHS",
            L10nType.ChineseTraditional => "CHT",
            _ => throw new ArgumentOutOfRangeException(nameof(self), self, null),
        };

        return $"{prefix}_{suffix}";
    }
}
