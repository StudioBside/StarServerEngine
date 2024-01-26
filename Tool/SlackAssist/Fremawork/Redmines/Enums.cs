namespace SlackAssist.Fremawork.Redmines
{
    internal static class Enums
    {
        // 혹시나 레드마인에 등록된 이슈 상태가 변경되거나 하면 여기를 수정해야합니다.
        public enum IssueStatusType
        {
            신규 = 1,
            진행,
            해결,
            의견,
            완료,
            거절,
            대기,
        }
    }
}
