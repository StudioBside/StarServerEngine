namespace Cs.Kafka
{
    using System;
    using System.Text;
    using Confluent.Kafka;

    using Cs.Exception;
    using Cs.Logging;
    using Cs.Messaging.Actor;
    using Cs.ServerEngine;
    using Cs.ServerEngine.Debug.Detail;

    public sealed class KafkaWriter : IActor<KafkaWriter>, IDisposable
    {
        private const int MaxSizeBytes = (1024 * 1024) - 100;
        private IProducer<string, string> producer = null!;
        private bool useTopicPrefix;
        private CaseSumCounter? counter;

        public KafkaWriter JobOwner => this;
        public IActorImplementor ActorImplementor { get; } = new ActorImplementor();
        public WriterReport Report { get; } = new WriterReport();

        public void Dispose()
        {
            this.producer.Dispose();
        }

        public void Initialize(string serverAddress, bool useTopicPrefix)
        {
            this.Post(self =>
            {
                this.useTopicPrefix = useTopicPrefix;

                var config = new ProducerConfig
                {
                    BootstrapServers = serverAddress,
                    ClientId = DnsUtil.HostName,
                };

                try
                {
                    this.producer = new ProducerBuilder<string, string>(config).Build();
                    if (this.producer == null)
                    {
                        ExceptionHandler.SendSlackWarning($"[Kafka] initialize fail. serverAddress:{serverAddress}");
                        return;
                    }

                    Log.Debug($"[GameLog] activate kafka writer. server:{serverAddress}");
                }
                catch (Exception ex)
                {
                    ExceptionHandler.SendSlackWarning($"[KafkaWriter] exception:{ex.Message}");
                }
            });
        }

        public void SetCounter(CaseSumCounter counter)
        {
            this.Post(self =>
            {
                self.counter = counter;
            });
        }

        public void Write(DateTime current, string topic, string key, string data)
        {
            var dataSize = Encoding.Default.GetByteCount(data);
            if (dataSize > MaxSizeBytes)
            {
                ExceptionHandler.SendSlackWarning($"[Kafka] Too large data. topic:{topic} key:{key} dataSize:{dataSize} data:{data.AsSpan(0, 20)}...");
                return;
            }

            this.Post(self => self.SendMessage(current, topic, key, data));
        }

        //// ------------------------------------------------------------------------------------------------------

        private void DeliveryHandler(DeliveryReport<string, string> report)
        {
            if (report.Error.Code != ErrorCode.NoError)
            {
                long increased = this.Report.IncreaseFailCount();
                Log.Error($"[Kafka] delieve failed. message:{report.Error.Reason} report:{this.Report} failCount:{increased}");
            }
        }

        private void SendMessage(DateTime current, string topic, string key, string data)
        {
            this.EnsureThreadSafe();

            ++this.Report.TotalRequest;
            Message<string, string> message = new()
            {
                Key = key,
                Value = data,
                Timestamp = new(current),
            };

            if (this.useTopicPrefix)
            {
                topic = $"{topic}-{DnsUtil.HostName}-{current:yyyy-MM-dd}";
            }
            else
            {
                topic = $"{topic}-{current:yyyy-MM-dd}";
            }

            topic = string.Intern(topic.ToLower());
            try
            {
                this.producer.Produce(topic, message, this.DeliveryHandler);
            }
            catch (Exception ex)
            {
                ExceptionHandler.SendSlackWarning($"[Kafka] topic:{topic} data:{data} exception:{ex.Message}");
            }

            this.counter?.AddCase();
        }
    }
}
