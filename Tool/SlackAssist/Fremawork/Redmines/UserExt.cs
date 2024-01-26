namespace SlackAssist.Fremawork.Redmines
{
    using System;
    using System.Collections.Generic;

    internal static class UserExt
    {
        public static string GetFullName(this global::Redmine.Net.Api.Types.User user)
        {
            return $"{user.LastName}{user.FirstName}";
        }
    }
}
