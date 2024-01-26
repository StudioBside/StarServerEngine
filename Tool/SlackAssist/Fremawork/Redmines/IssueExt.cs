namespace SlackAssist.Fremawork.Redmines;

using System.Web;
using global::Redmine.Net.Api.Types;
using static SlackAssist.Fremawork.Redmines.Enums;

internal static class IssueExt
{
    public static string GetEncodedSubject(this Issue self)
    {
        return HttpUtility.HtmlEncode(self.Subject);
    }

    public static bool IsCompleted(this Issue self)
    {
        return self.Status.Id == (int)IssueStatusType.완료;
    }
}
