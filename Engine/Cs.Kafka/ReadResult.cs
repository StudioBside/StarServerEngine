namespace Cs.Kafka
{
    using Confluent.Kafka;

    public readonly struct ReadResult
    {
        private readonly IConsumer<string, string> consumer;
        private readonly ConsumeResult<string, string> consumeResult;

        public ReadResult(IConsumer<string, string> consumer, ConsumeResult<string, string> consumeResult)
        {
            this.consumer = consumer;
            this.consumeResult = consumeResult;
        }

        public string Topic => this.consumeResult.TopicPartitionOffset.Topic;
        public Partition Partition => this.consumeResult.TopicPartitionOffset.Partition;
        public Offset Offset => this.consumeResult.TopicPartitionOffset.Offset;
        public string Value => this.consumeResult.Message.Value;

        public void CommitAll()
        {
            this.consumer.Commit();
        }
    }
}
