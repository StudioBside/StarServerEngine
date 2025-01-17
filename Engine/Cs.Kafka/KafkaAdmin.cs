namespace Cs.Kafka;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

public sealed class KafkaAdmin : IDisposable
{
    private readonly IAdminClient adminClient;
    private readonly string bootstrapServers;

    private KafkaAdmin(IAdminClient adminClient, string bootstrapServers)
    {
        this.adminClient = adminClient;
        this.bootstrapServers = bootstrapServers;
    }

    public string Url => this.bootstrapServers;

    public static bool Create(string bootstrapServers, [MaybeNullWhen(false)] out KafkaAdmin admin)
    {
        if (string.IsNullOrEmpty(bootstrapServers))
        {
            admin = null;
            return false;
        }

        try
        {
            var config = new AdminClientConfig
            { 
                BootstrapServers = bootstrapServers,
            };

            var client = new AdminClientBuilder(config).Build();
            admin = new KafkaAdmin(client, bootstrapServers);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create KafkaAdmin: {ex.Message}");
            admin = null;
            return false;
        }
    }

    public static TopicMetadata? GetTopicMetadata(string servers, string topicName)
    {
        if (!Create(servers, out var admin))
        {
            return null;
        }

        return admin.GetTopicMetadata(topicName);
    }

    public void PrintMetadata(string servers)
    {
        // Warning: The API for this functionality is subject to change.
        var meta = this.adminClient.GetMetadata(TimeSpan.FromSeconds(20));
        Console.WriteLine($"{meta.OriginatingBrokerId} {meta.OriginatingBrokerName}");
        meta.Brokers.ForEach(broker =>
            Console.WriteLine($"Broker: {broker.BrokerId} {broker.Host}:{broker.Port}"));

        meta.Topics.ForEach(topic =>
        {
            Console.WriteLine($"Topic: {topic.Topic} {topic.Error}");
            topic.Partitions.ForEach(partition =>
            {
                Console.WriteLine($"  Partition: {partition.PartitionId}");
                Console.WriteLine($"    Replicas: {ToString(partition.Replicas)}");
                Console.WriteLine($"    InSyncReplicas: {ToString(partition.InSyncReplicas)}");
            });
        });

        static string ToString(int[] array) => $"[{string.Join(", ", array)}]";
    }

    public IReadOnlyList<GroupInfo> GetGroups()
    {
        // Warning: The API for this functionality is subject to change.
        return this.adminClient.ListGroups(TimeSpan.FromSeconds(10));
    }

    public HashSet<string> GetTopics()
    {
        var meta = this.adminClient.GetMetadata(TimeSpan.FromSeconds(20));
        return meta.Topics.Select(e => e.Topic).ToHashSet();
    }

    public void Dispose()
    {
        this.adminClient.Dispose();
    }

    public bool IsTopicEmpty(string topicName)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = this.bootstrapServers,
            GroupId = Guid.NewGuid().ToString(),
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topicName);

        try
        {
            var partitions = consumer.Assignment;
            var watermarks = new Dictionary<TopicPartition, WatermarkOffsets>();

            foreach (var partition in consumer.Assignment)
            {
                var watermark = consumer.QueryWatermarkOffsets(partition, TimeSpan.FromSeconds(5));
                watermarks[partition] = watermark;
            }

            // 모든 파티션의 시작과 끝 오프셋이 같으면 토픽이 비어있음
            return watermarks.All(w => w.Value.Low == w.Value.High);
        }
        finally
        {
            consumer.Close();
        }
    }

    public async Task<bool> DeleteTopicsAsync(IEnumerable<string> topicNames)
    {
        try
        {
            await this.adminClient.DeleteTopicsAsync(topicNames);
            return true;
        }
        catch (DeleteTopicsException ex)
        {
            Console.WriteLine($"토픽 삭제 중 오류 발생: {ex.Message}");
            return false;
        }
    }

    public TopicInfo? GetTopicInfo(string topicName)
    {
        var metadata = this.adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(10));
        var topicMetadata = metadata.Topics.FirstOrDefault();

        if (topicMetadata == null)
        {
            return null;
        }

        return new TopicInfo(topicName, topicMetadata.Partitions.Count, this.IsTopicEmpty(topicName));
    }
    
    //// ---------------------------------------------------------

    private TopicMetadata? GetTopicMetadata(string topicName)
    {
        var meta = this.adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(20));
        return meta.Topics.FirstOrDefault();
    }

    public sealed record TopicInfo(string Name, int PartitionCount, bool IsEmpty);
}
