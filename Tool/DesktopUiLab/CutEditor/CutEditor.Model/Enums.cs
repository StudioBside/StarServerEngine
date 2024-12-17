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

    public enum TransitionControl
    {
        NONE,
        IN,
        OUT,
    }

    public enum BgFadeType
    {
        FadeIn,
        FadeOut,
    }

    public enum Ease
    {
        Unset,
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        Flash,
        InFlash,
        OutFlash,
        InOutFlash,
    }

    public enum EaseCategory
    {
        All,
        In,
        Out,
        InOut,
    }

    public enum CameraOffsetCategory
    {
        None,
        Default,
        All,
        One,
        Twin,
        Triple,
        Pos2x,
    }

    public enum L10nMappingType
    {
        Normal,
        MissingImported, // 원본에 있으나 번역본에 없음 (원본이 추가된 경우)
        MissingOrigin, // 원본에 없으나 번역본에 있음 (원본이 삭제된 경우)
        TextChanged, // 원본과 번역본이 다름
    }

    public enum SearchCombinationType
    {
        And,
        Or,
    }

    public static bool IsIn(this Ease self)
    {
        return self switch
        {
            Ease.InSine => true,
            Ease.InQuad => true,
            Ease.InCubic => true,
            Ease.InQuart => true,
            Ease.InQuint => true,
            Ease.InExpo => true,
            Ease.InCirc => true,
            Ease.InElastic => true,
            Ease.InBack => true,
            Ease.InBounce => true,
            Ease.InFlash => true,
            _ => false,
        };
    }

    public static bool IsOut(this Ease self)
    {
        return self switch
        {
            Ease.OutSine => true,
            Ease.OutQuad => true,
            Ease.OutCubic => true,
            Ease.OutQuart => true,
            Ease.OutQuint => true,
            Ease.OutExpo => true,
            Ease.OutCirc => true,
            Ease.OutElastic => true,
            Ease.OutBack => true,
            Ease.OutBounce => true,
            Ease.OutFlash => true,
            _ => false,
        };
    }

    public static bool IsInOut(this Ease self)
    {
        return self switch
        {
            Ease.InOutSine => true,
            Ease.InOutQuad => true,
            Ease.InOutCubic => true,
            Ease.InOutQuart => true,
            Ease.InOutQuint => true,
            Ease.InOutExpo => true,
            Ease.InOutCirc => true,
            Ease.InOutElastic => true,
            Ease.InOutBack => true,
            Ease.InOutBounce => true,
            Ease.InOutFlash => true,
            _ => false,
        };
    }
}
