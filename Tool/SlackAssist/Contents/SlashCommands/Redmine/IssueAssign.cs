namespace SlackAssist.Contents.SlashCommands.Redmine;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cs.Logging;
using Cs.Messaging;
using SlackAssist.Configs;
using SlackAssist.Fremawork.Redmines;
using SlackAssist.Fremawork.Slack;
using SlackAssist.Fremawork.Workflow;
using SlackAssist.SlackNetHandlers;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

internal sealed class IssueAssign : ISlashSubCommand, IWorkflowCommand
{
    public string Command => SlashCommandHandler.BuildCommand("redmine");
    public IEnumerable<string> CommandLiterals { get; set; } = new[] { "assign" };

    public Block GetIntroduceBlock()
    {
        using var builder = new MarkdownBuilder();
        builder.WriteLine($"명령 : {string.Join(", ", this.CommandLiterals.Select(e => $"`{e}`"))} :redmine:");
        builder.WriteLine($"효과 : 레드마인의 일감 할당 상황을 요약하여 알려줍니다.");
        builder.WriteLine($"문법 : <서브명령>");
        builder.WriteLine($"예시 : *{this.Command} assign*");

        return builder.FlushToSectionBlock();
    }

    public async Task<Message> Process(ISlackApiClient slack, IReadOnlyList<string> arguments)
    {
        var result = new Message
        {
            IconEmoji = ":redmine:",
            Username = "Readmine",
        };

        var allUsers = Redmine.Instance.Users.ToDictionary(e => e.Id);

        var workingUserIds = new HashSet<int>();

        using var versionBuilder = new MarkdownBuilder();
        // 현재 열려있는 버전만을 대상으로 한다.
        foreach (var version in Redmine.Instance.OpenedVersions.OrderBy(e => e.Name))
        {
            var issues = await version.GetIssues(statusId: "open");
            if (issues is null || issues.Any() == false)
            {
                versionBuilder.WriteLine($"{version.Name} : 현재 진행중인 일감이 없습니다");
                continue;
            }

            var workers = new Dictionary<int /*userId*/, global::Redmine.Net.Api.Types.User>();
            foreach (var issue in issues)
            {
                if (issue.AssignedTo is null)
                {
                    continue;
                }

                int userId = issue.AssignedTo.Id;
                if (workingUserIds.Contains(userId))
                {
                    continue;
                }

                if (allUsers.TryGetValue(userId, out var user) == false)
                {
                    Log.Warn($"작업자를 찾을 수 없습니다. userId:{userId}");
                    continue;
                }

                Log.Debug($" - 작업자 추가:{user.GetFullName()} version:{version.Name}");

                workers.Add(userId, user);
                workingUserIds.Add(userId);
            }

            versionBuilder.WriteLine($"{version.Name} : 일감 {issues.Count}개, 작업자 {workers.Count}명 참여중");
        }

        // 이슈가 할당되지 않은 작업자를 찾는다
        var idleWorkers = new List<global::Redmine.Net.Api.Types.User>();
        var config = SlackAssistConfig.Instance.Redmine;
        foreach (var user in allUsers.Values)
        {
            if (config.InvisiableUsers.Any(e => user.GetFullName().Contains(e)))
            {
                continue;
            }

            if (workingUserIds.Contains(user.Id))
            {
                continue;
            }

            idleWorkers.Add(user);
        }

        using var idleWorkerBuilder = new MarkdownBuilder();
        DataTable idleWorkerTable = new DataTable();
        if (idleWorkers.Any() == false)
        {
            idleWorkerBuilder.WriteLine($"모든 작업자가 일감을 진행하고 있습니다.");
        }
        else
        {
            idleWorkerBuilder.WriteLine($"미할당 작업자 : {idleWorkers.Count}명");

            var groups = idleWorkers.GroupBy(e => e.Groups?.FirstOrDefault()).ToDictionary(e => e.Key?.Name ?? "그룹 없음");
            foreach (var group in groups.OrderBy(e => e.Key))
            {
                idleWorkerBuilder.WriteLine($"*{group.Key} :*");

                var userList = group.Value.Select(e => $"{e.GetFullName()}");
                idleWorkerBuilder.WriteLine($"> {string.Join(", ", userList)}");
            }
        }

        // 결과 만들기
        result.Blocks.Add(new HeaderBlock { Text = $"일감 할당 상황 {workingUserIds.Count}/{allUsers.Count}명" });
        result.Blocks.Add(versionBuilder.FlushToSectionBlock());
        result.Blocks.Add(idleWorkerBuilder.FlushToSectionBlock());

        return result;
    }

    public Task<Message> Process(ISlackApiClient slack, SlashCommand command, IReadOnlyList<string> arguments)
    {
        Task.Run(async () =>
        {
            var message = await this.Process(slack, arguments);
            message.Channel = command.ChannelId;
            await slack.Chat.PostEphemeral(command.UserId, message);
        });

        return Task.FromResult(new Message { Text = ":loading:" });
    }
}