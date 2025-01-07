#nullable enable

namespace NKM
{
    using System.ComponentModel;

    public static class NKMOpenEnums
    {
        public enum NKM_UNIT_TYPE : short
        {
            NUT_INVALID = 0,
            NUT_SYSTEM,     // 전투 가능. 덱템플릿 등에서 내가 소유하지 않은 유닛을 사용해 전투할 때. 튜토리얼 등에 쓰임.
            NUT_SAVIOR,     // 구원자 (플레이 유닛)
            NUT_ENV,        // 환경유닛, 더미
            NUT_MONSTER,    // 몬스터
            NUT_RAID_MONSTER,  // 레이드 몬스터
            NUT_MERCENARY,  // 서포터 (여정 플레이 불가능, 여정 중 영입 가능, 여정 전투 참여 가능)
            NUT_NPC,        // NPC (컷씬에서만 등장)
            NUT_ORBITAL_ARRAY,  // 오비탈 스킬에 사용하는 유닛
        }

        public enum UnitIdConst
        {
            [Description("구원자")]
            CURRENT_CHARACTER,
            [Description("부모1")]
            CURRENT_PARENT1,
            [Description("부모2")]
            CURRENT_PARENT2,
        }

        //// -----------------------------------------------------------------------------------------------

        public static bool BattleEnable(this NKM_UNIT_TYPE unitType)
        {
            return unitType == NKM_UNIT_TYPE.NUT_SAVIOR ||
                unitType == NKM_UNIT_TYPE.NUT_MONSTER ||
                unitType == NKM_UNIT_TYPE.NUT_RAID_MONSTER ||
                unitType == NKM_UNIT_TYPE.NUT_MERCENARY;
        }
    }
}