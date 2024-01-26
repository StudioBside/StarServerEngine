namespace SlackAssist.Fremawork.Redmines;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Cs.Logging;
using global::Redmine.Net.Api;
using global::Redmine.Net.Api.Async;
using global::Redmine.Net.Api.Types;

internal sealed class RedmineVersion
{
    private readonly Redmine redmine;
    private readonly Version version;

    public RedmineVersion(Redmine redmine, Version version)
    {
        this.redmine = redmine;
        this.version = version;
    }

    public int ProjectId => this.version.Project.Id;
    public int VersionId => this.version.Id;
    public bool IsOpened => this.version.Status == VersionStatus.Open;
    public string Name => this.version.Name;
    private string DebugName => $"[RedmineVersion:{this.Name}]";

    public async Task<IReadOnlyList<Issue>> GetIssues(string statusId)
    {
        var parameters = new NameValueCollection
        {
            { RedmineKeys.STATUS_ID, statusId },
            { RedmineKeys.PROJECT_ID, this.ProjectId.ToString() },
            { RedmineKeys.FIXED_VERSION_ID, this.VersionId.ToString() },
        };

        try
        {
            return await this.redmine.Manager.GetObjectsAsync<Issue>(parameters);
        }
        catch (System.Exception e)
        {
            Log.Error($"{this.DebugName} statusId:{statusId} {e.Message}");
            return System.Array.Empty<Issue>();
        }
    }
}
