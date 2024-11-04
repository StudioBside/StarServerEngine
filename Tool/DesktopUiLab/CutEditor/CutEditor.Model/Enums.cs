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
}
