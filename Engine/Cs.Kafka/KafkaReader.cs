namespace Cs.Kafka
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Cs.Exception;
    using Cs.Logging;
    using Cs.Messaging;

    public sealed class KafkaReader : IDisposable
    {
        private readonly TaskCompletionSource<bool> tcs = new();
        private IConsumer<string, string> consumer = null!;
        private Action<ReadResult> handler = null!;
        private long fetchCount;
        private string serverAddress = null!;

        public void Dispose()
        {
            this.consumer.Close();
            this.consumer.Dispose();
        }

        public bool Initialize(string serverAddress, string groupId, bool autoCommit)
        {
            this.serverAddress = serverAddress;

            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = this.serverAddress,
                EnableAutoCommit = autoCommit,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            try
            {
                this.consumer = new ConsumerBuilder<string, string>(config).Build();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.SendSlackWarning($"[KafkaReader] exception:{ex.Message}");
                return false;
            }
        }

        public Task<bool> ReadAsync(string topic, Action<ReadResult> handler)
        {
            this.handler = handler;

            var metaData = KafkaAdmin.GetTopicMetadata(this.serverAddress, topic);
            if (metaData == null)
            {
                Log.Error($"[KafkaReader] get topic metadata failed. topic:{topic}");
                return Task.FromResult(false);
            }

            Log.Debug($"[KafkaReader] start read. topic:{topic} #partitions:{metaData.Partitions.Count}");

            this.consumer.Assign(metaData.Partitions.Select(e => new TopicPartition(topic, e.PartitionId)));
            BackgroundJob.Execute(this.ConsumeLoop);
            return this.tcs.Task;
        }

        public bool Read(string topic, Action<ReadResult> handler)
        {
            this.handler = handler;

            var metaData = KafkaAdmin.GetTopicMetadata(this.serverAddress, topic);
            if (metaData == null)
            {
                Log.Error($"[KafkaReader] get topic metadata failed. topic:{topic}");
                return false;
            }

            Log.Debug($"[KafkaReader] start read. topic:{topic} #partitions:{metaData.Partitions.Count}");

            this.consumer.Assign(metaData.Partitions.Select(e => new TopicPartition(topic, e.PartitionId)));
            this.ConsumeLoop();
            return this.tcs.Task.Result;
        }

        public void Commit()
        {
            this.consumer.Commit();
        }

        public async Task ReadFromBeginningAsync(string topic, int messageCount, Action<ReadResult> handler)
        {
            await Task.Delay(0);

            this.handler = handler;

            var metaData = KafkaAdmin.GetTopicMetadata(this.serverAddress, topic);
            if (metaData == null)
            {
                Log.Error($"[KafkaReader] get topic metadata failed. topic:{topic}");
                return;
            }

            Log.Debug($"[KafkaReader] start read from beginning. topic:{topic} #partitions:{metaData.Partitions.Count}");

            this.consumer.Assign(metaData.Partitions.Select(e => new TopicPartition(topic, e.PartitionId)));

            int readMessages = 0;
            try
            {
                while (readMessages < messageCount)
                {
                    var result = this.consumer.Consume(TimeSpan.FromSeconds(2));
                    if (result == null)
                    {
                        Log.Debug($"[KafkaReader] consume timeout. fetchCount:{this.fetchCount}");
                        break;
                    }

                    ++this.fetchCount;
                    ++readMessages;
                    this.handler.Invoke(new(this.consumer, result));
                }
            }
            catch (ConsumeException ex)
            {
                Log.Error($"[KafkaReader] consumeError. message:{ex.Message}");
            }
            catch (OperationCanceledException ex)
            {
                Log.Debug($"[KafkaReader] canceled. message:{ex.Message}");
            }
        }
        
        //// ------------------------------------------------------------------------------------------------------

        private void ConsumeLoop()
        {
            try
            {
                while (true)
                {
                    ConsumeResult<string, string>? result = this.consumer.Consume(TimeSpan.FromSeconds(2));
                    if (result == null)
                    {
                        Log.Debug($"[KafkaReader] consume timeout. fetchCount:{this.fetchCount}");
                        this.tcs.SetResult(true);
                        return;
                    }

                    ++this.fetchCount;
                    this.handler.Invoke(new(this.consumer, result));
                }
            }
            catch (ConsumeException ex)
            {
                Log.Error($"[KafkaReader] consumeError. message:{ex.Message}");
            }
            catch (OperationCanceledException ex)
            {
                Log.Debug($"[KafkaReader] canceled. message:{ex.Message}");
            }

            this.tcs.SetResult(false);
        }
    }
}
