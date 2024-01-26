namespace SlackAssist.Contents.Detail
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Redmine.Net.Api.Types;

    internal static class RedmineController
    {
        public static async Task<List<Issue>> GetAllIssueInVersion(string keyword)
        {
            var redmine = Redmine.Instance;

            var versions = redmine.GetVersions(keyword);
            if (versions.Any() == false)
            {
                return new List<Issue>();
            }

            var issues = new List<Issue>();
            foreach (var version in versions)
            {
                var versionIssues = await version.GetIssues(statusId: "*");
                issues.AddRange(versionIssues);
            }

            return issues;
        }
    }
}
