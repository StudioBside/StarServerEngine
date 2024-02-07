namespace SlackAssist.Fremawork.Redmines;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Cs.Core;
using Cs.Logging;
using global::Redmine.Net.Api;
using global::Redmine.Net.Api.Async;
using global::Redmine.Net.Api.Types;
using SlackAssist;
using SlackAssist.Configs;

internal sealed class Redmine
{
    private readonly Dictionary<string /*한글 이름*/, User> users = new();
    private readonly Dictionary<int /*projectId*/, Project> projects = new();
    private readonly List<RedmineVersion> versions = new();
    private RedmineManager manager = null!;

    public static Redmine Instance => Singleton<Redmine>.Instance;
    public string Host => this.manager.Host;
    public IEnumerable<RedmineVersion> OpenedVersions => this.versions.Where(e => e.IsOpened);
    internal IEnumerable<User> Users => this.users.Values;
    internal RedmineManager Manager => this.manager;
    private static string DebugName => $"[RedmineController]";

    public async Task Initialze(SlackAssistConfig.RedmineConfig config)
    {
        this.manager = new RedmineManager(config.ServerAddress, config.ApiKey);

        // ----- 프로젝트 목록 가져오기 --------------------------------------------------
        this.projects.Clear();
        var projects = await this.manager.GetObjectsAsync<Project>(new NameValueCollection());
        if (projects is null)
        {
            return;
        }

        foreach (var project in projects)
        {
            this.projects.Add(project.Id, project);
        }

        Log.Debug($"{DebugName} project count:{this.projects.Count}");

        // ----- 버전 목록 가져오기 --------------------------------------------------
        this.versions.Clear();
        foreach (var project in this.projects)
        {
            var versionParameter = new NameValueCollection
            {
                { RedmineKeys.PROJECT_ID, project.Key.ToString() },
            };

            // 버전 목록 가져오기 (파라메터에 프로젝트 2개 이거나 없으면 에러)
            var versions = await this.manager.GetObjectsAsync<global::Redmine.Net.Api.Types.Version>(versionParameter);
            if (versions is null)
            {
                continue;
            }

            foreach (var version in versions)
            {
                this.versions.Add(new RedmineVersion(this, version));
            }
        }

        int openedVersionCount = this.versions.Count(e => e.IsOpened);
        Log.Debug($"{DebugName} version count:{this.versions.Count} (#opened:{openedVersionCount})");

        // ----- 유저 목록 가져오기 --------------------------------------------------
        var parameters = new NameValueCollection
        {
            { RedmineKeys.STATUS, "1" },    // 활성화 된 유저만 가져오기
            { RedmineKeys.LIMIT, "300" },   // 지정하지 않으면 기본 25라서 임의로 300으로 정함
        };

        // 유저 목록 가져오기
        var users = await this.manager.GetObjectsAsync<User>(parameters);
        foreach (var userId in users.Select(e => e.Id))
        {
            // 그룹과 멤버쉽이 포함된 정보 가져오기
            var perUserParameters = new NameValueCollection
            {
                { RedmineKeys.INCLUDE, $"{RedmineKeys.MEMBERSHIPS},{RedmineKeys.GROUPS}" },
            };

            var user = await this.manager.GetObjectAsync<User>(userId.ToString(), perUserParameters);

            // 이름이 중복인 경우는 처리하지 않음
            this.users.Add($"{user.GetFullName()}", user);
        }

        Log.Debug($"{DebugName} user count:{this.users.Count}");
    }

    public Project? GetProjects(int projectId)
    {
        this.projects.TryGetValue(projectId, out var result);
        return result;
    }

    public async Task<Issue?> GetIssue(string issueId)
    {
        try
        {
            return await this.manager.GetObjectAsync<Issue>(issueId, parameters: null);
        }
        catch (Exception ex)
        {
            Log.Warn($"{DebugName} invalid issueId:{issueId} exception:{ex.Message}");
            return null;
        }
    }

    public IEnumerable<RedmineVersion> GetVersions(string keyword)
    {
        return this.versions.Where(e => e.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase));
    }

    public IssueUpdater GetIssueUpdater(Issue issue) => new IssueUpdater(this.manager, issue);

    public bool GetUser(string name, [MaybeNullWhen(false)] out User user)
    {
        return this.users.TryGetValue(name, out user);
    }
}