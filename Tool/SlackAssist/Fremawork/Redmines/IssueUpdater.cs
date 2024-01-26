namespace SlackAssist;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cs.Logging;

using global::Redmine.Net.Api;
using global::Redmine.Net.Api.Async;
using global::Redmine.Net.Api.Types;
using static SlackAssist.Fremawork.Redmines.Enums;

internal sealed partial class IssueUpdater(RedmineManager manager, Issue issue) : IAsyncDisposable
{
    private readonly Issue issue = issue;
    private readonly RedmineManager manager = manager;

    private string DebugName => $"[IssueUpdater:{this.issue.Id}]";

    public void AddLinkDescription(string description)
    {
        if (string.IsNullOrEmpty(this.issue.Description))
        {
            this.issue.Description = $"[SlackAssist.Start]\n{description}\n[SlackAssist.End]";
            return;
        }

        var match = SectionRegex().Match(this.issue.Description);

        this.issue.Description = match.Success
            ? SectionRegex().Replace(this.issue.Description, $"[SlackAssist.Start]\n{description}\n[SlackAssist.End]")
            : $"[SlackAssist.Start]\n{description}\n[SlackAssist.End]\n<hr>\n\n{this.issue.Description}"; // 230113. <hr> 뒤에 개행 두 번 하지 않으면 이후 줄바꿈이 안된 채로 표시된다. 

        Log.Debug($"{this.DebugName} update redmine description.");
    }

    public async Task<bool> ChangeStaus(IssueStatusType status)
    {
        var statuses = await this.manager.GetObjectsAsync<IssueStatus>(parameters: null);
        if (statuses is null)
        {
            return false;
        }

        var foundStatus = statuses.FirstOrDefault(e => e.Name.Contains(status.ToString()));
        if (foundStatus is null)
        {
            return false;
        }

        Log.Debug($"{this.DebugName} update redmine status. {this.issue.Status.Name} -> {foundStatus.Name}");
        this.issue.Status = IdentifiableName.Create<IssueStatus>(foundStatus.Id);

        return true;
    }

    public bool ChangeAssignUser(string name)
    {
        if (Redmine.Instance.GetUser(name, out var user) == false)
        {
            return false;
        }

        if (this.issue.AssignedTo?.Name == name)
        {
            return true;
        }

        var newAssigendTo = IdentifiableName.Create<IdentifiableName>(user.Id);
        newAssigendTo.Name = name;

        Log.Debug($"{this.DebugName} update redmine assign user. {this.issue.AssignedTo?.Name} -> {name}");
        this.issue.AssignedTo = newAssigendTo;

        return true;
    }

    public void ChangeAssignUser(IdentifiableName assignUser)
    {
        this.issue.AssignedTo = assignUser;

        Log.Debug($"{this.DebugName} update redmine assign user.");
    }

    public void AddNotes(string notes)
    {
        this.issue.Notes = $"{notes}\n";
        Log.Debug($"{this.DebugName} add redmine notes.");
    }

    public async ValueTask DisposeAsync()
    {
        await this.manager.UpdateObjectAsync(this.issue.Id.ToString(), this.issue);
    }

    [GeneratedRegex(@"\[SlackAssist\.Start\].*\[SlackAssist\.End\]", RegexOptions.Singleline)]
    private static partial Regex SectionRegex();
}