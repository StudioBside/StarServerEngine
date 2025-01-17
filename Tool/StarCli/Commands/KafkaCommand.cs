namespace StarCli.Commands;

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using Cs.Cli;
using Cs.Kafka;
using Cs.Logging;

internal sealed class KafkaCommand : CommandBase, IDisposable
{
    public KafkaCommand(IHomePathConfig config) : base(config, "kafka", "카프카 제어")
    {
        var optMaxCount = new Option<int>(["--max-count", "-m"], () => 20, "토픽(혹은 데이터)의 최대 개수를 제한합니다");
        this.AddGlobalOption(optMaxCount);

        var argTopic = new Argument<string>("topic", () => string.Empty, "토픽 이름을 지정합니다");
        var subCommand = new Command("info", "카프카 정보를 조회합니다") { argTopic };
        subCommand.SetHandler(this.OnRecvInfo, argTopic);
        this.AddCommand(subCommand);

        var optEmpty = new Option<bool>(["--empty", "-e"], "비어있는 토픽만을 대상으로 합니다");
        subCommand = new Command("topics", "토픽 목록을 조회합니다") { optEmpty };
        subCommand.SetHandler(this.OnRecvTopics, optEmpty, optMaxCount);
        this.AddCommand(subCommand);

        subCommand = new Command("prune", "비어있는 토픽을 모두 삭제합니다");
        subCommand.SetHandler(this.OnRecvPrune, optMaxCount);
        this.AddCommand(subCommand);

        subCommand = new Command("sampling", "토픽 데이터를 샘플링합니다");
        subCommand.SetHandler(this.OnRecvSampling, optMaxCount);
        this.AddCommand(subCommand);

        subCommand = new Command("groups", "그룹 목록을 조회합니다");
        subCommand.SetHandler(this.OnRecvGroups, optMaxCount);
        this.AddCommand(subCommand);
    }

    public void Dispose()
    {
        Log.Info("KafkaCommand 해제");
    }

    private void OnRecvGroups(int maxCount)
    {
        if (this.CreateAdmin(out var admin) == false)
        {
            return;
        }

        Log.Info($"그룹 목록을 조회합니다 maxCount:{maxCount}");
        var groups = admin.GetGroups();
        if (groups.Count == 0)
        {
            Log.Info("그룹이 없습니다");
            return;
        }

        foreach (var info in groups.Take(maxCount))
        {
            Log.Info($"  Group: {info.Group} {info.Error} {info.State}");
            Log.Info($"  Broker: {info.Broker.BrokerId} {info.Broker.Host}:{info.Broker.Port}");
            Log.Info($"  Protocol: {info.ProtocolType} {info.Protocol}");
            Log.Info($"  Members:");
            foreach (var member in info.Members)
            {
                Log.Info($"    {member.MemberId} {member.ClientId} {member.ClientHost}");
                Log.Info($"    Metadata: {member.MemberMetadata.Length} bytes");
                Log.Info($"    Assignment: {member.MemberAssignment.Length} bytes");
            }
        }
    }

    private void OnRecvSampling(int obj)
    {
    }

    private void OnRecvTopics(bool emptyOnly, int maxCount)
    {
        if (this.CreateAdmin(out var admin) == false)
        {
            return;
        }

        Log.Info($"토픽 목록을 조회합니다 maxCount:{maxCount} emptyOnly:{emptyOnly}");

        var topics = admin.GetTopics();
        int index = 0;
        foreach (var topic in topics)
        {
            if (emptyOnly && admin.IsTopicEmpty(topic) == false)
            {
                continue;
            }

            Log.Info($"[index:{index}] {topic}");
            index++;

            if (index >= maxCount)
            {
                break;
            }
        }
    }

    private async Task OnRecvPrune(int maxCount)
    {
        if (this.CreateAdmin(out var admin) == false)
        {
            return;
        }

        Log.Info($"값이 없는 빈 토픽을 삭제합니다 maxCount:{maxCount}");

        var topics = admin.GetTopics();
        if (topics.Count == 0)
        {
            Log.Info("토픽이 없습니다");
            return;
        }

        var targetTopics = topics.Where(admin.IsTopicEmpty)
            .Take(maxCount)
            .ToList();

        if (targetTopics.Count == 0)
        {
            Log.Info($"삭제할 토픽이 없습니다. 총 토픽의 개수:{topics.Count}");
            return;
        }

        if (await admin.DeleteTopicsAsync(targetTopics) == false)
        {
            Log.Error("토픽 삭제 중 오류가 발생했습니다");
            return;
        }

        for (int i = 0; i < targetTopics.Count; i++)
        {
            var topic = targetTopics[i];
            Log.Info($"[index:{i}] {topic}");
        }
    }

    private void OnRecvInfo(string topicName)
    {
        if (this.CreateAdmin(out var admin) == false)
        {
            return;
        }

        if (string.IsNullOrEmpty(topicName))
        {
            Log.Info($"카프카 정보: {admin.Url}");
            return;
        }

        var topicInfo = admin.GetTopicInfo(topicName);
        if (topicInfo is null)
        {
            Log.Info($"토픽 '{topicName}'을 찾을 수 없습니다");
            return;
        }

        Log.Info($" - 이름:{topicInfo.Name}");
        Log.Info($" - 파티션 수:{topicInfo.PartitionCount}");
        Log.Info($" - 비어있음:{topicInfo.IsEmpty}");
    }

    private bool CreateAdmin([MaybeNullWhen(false)] out KafkaAdmin admin)
    {
        var url = this.GetBranchConfig("kafka");
        if (url is null)
        {
            admin = null;
            return false;
        }

        return KafkaAdmin.Create(url, out admin);
    }
}
