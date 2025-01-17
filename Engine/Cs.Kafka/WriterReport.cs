namespace Cs.Kafka
{
    using System.Threading;

    public sealed class WriterReport
    {
        private long delieverFailed;

        public long TotalRequest { get; internal set; }
        public long DelieverFailed => this.delieverFailed;
        public long DelieverSuccess => this.TotalRequest - this.DelieverFailed;

        public override string ToString()
        {
            float failRate = this.delieverFailed * 100f / this.TotalRequest;
            return $"[KafkaWriter] success:{this.DelieverSuccess}/{this.TotalRequest} failRate:{failRate:0.##}%";
        }

        internal long IncreaseFailCount()
        {
            return Interlocked.Increment(ref this.delieverFailed); // ret : incremented value
        }
    }
}
