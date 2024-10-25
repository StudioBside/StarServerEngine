#nullable enable

namespace CutEditor.Model
{
    public static class NKMCutsceneEnums
    {
        public enum JumpAnchorType
        {
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
            None,
        }

        public enum RewardAnchorType
        {
            REWARD_ANCHOR_1,
            REWARD_ANCHOR_2,
            REWARD_ANCHOR_3,
            REWARD_ANCHOR_4,
            REWARD_ANCHOR_5,
            None,
        }

        public static JumpAnchorType GetCategory(this JumpAnchorType jumpAnchorType)
        {
            return jumpAnchorType switch
            {
                JumpAnchorType.JUMPANCHOR_1 or
                JumpAnchorType.JUMPANCHOR_1_SUCCESS or
                JumpAnchorType.JUMPANCHOR_1_FAIL => JumpAnchorType.JUMPANCHOR_1,
                JumpAnchorType.JUMPANCHOR_2 or
                JumpAnchorType.JUMPANCHOR_2_SUCCESS or
                JumpAnchorType.JUMPANCHOR_2_FAIL => JumpAnchorType.JUMPANCHOR_2,
                JumpAnchorType.JUMPANCHOR_3 or
                JumpAnchorType.JUMPANCHOR_3_SUCCESS or
                JumpAnchorType.JUMPANCHOR_3_FAIL => JumpAnchorType.JUMPANCHOR_3,
                JumpAnchorType.JUMPANCHOR_4 or
                JumpAnchorType.JUMPANCHOR_4_SUCCESS or
                JumpAnchorType.JUMPANCHOR_4_FAIL => JumpAnchorType.JUMPANCHOR_4,
                JumpAnchorType.JUMPANCHOR_5 or
                JumpAnchorType.JUMPANCHOR_5_SUCCESS or
                JumpAnchorType.JUMPANCHOR_5_FAIL => JumpAnchorType.JUMPANCHOR_5,
                _ => jumpAnchorType,
            };
        }

        public static bool IsBasicJumpType(this JumpAnchorType jumpAnchorType)
        {
            return jumpAnchorType == jumpAnchorType.GetCategory();
        }

        public static bool IsExtendJumpType(this JumpAnchorType jumpAnchorType)
        {
            return jumpAnchorType != jumpAnchorType.GetCategory();
        }
    }
}