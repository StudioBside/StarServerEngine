namespace Cs.Math.Lottery
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public sealed class RatioLottery<T> : IReadOnlyList<T>
    {
        private readonly List<CaseData> cases = new List<CaseData>();
        private int totalRatio;

        public int TotalRatio => this.totalRatio;
        public int Count => this.cases.Count;

        public IEnumerable<CaseData> Cases => this.cases;
        public IEnumerable<T> CaseValues => this.cases.Select(e => e.Value);
        public T this[int index] => this.cases[index].Value;

        public void AddCase(int ratio, T value)
        {
            if (ratio <= 0)
            {
                throw new Exception($"invalid ratio data. ratio:{ratio}");
            }

            this.totalRatio += ratio;
            this.cases.Add(new CaseData(ratio, this.totalRatio, value));
        }

        public T Decide()
        {
            int randomValue = RandomGenerator.Next(this.TotalRatio);
            foreach (CaseData data in this.cases)
            {
                if (randomValue < data.AccumulatedRatio)
                {
                    return data.Value;
                }
            }

            throw new Exception($"[RatioLottery] pick failed. randomValue:{randomValue} totalRatio:{this.totalRatio}");
        }

        public T Decide(Random random)
        {
            int randomValue = random.Next(this.TotalRatio);
            foreach (CaseData data in this.cases)
            {
                if (randomValue < data.AccumulatedRatio)
                {
                    return data.Value;
                }
            }

            throw new Exception($"[RatioLottery] pick failed. randomValue:{randomValue} totalRatio:{this.totalRatio}");
        }

        public IEnumerator<T> GetEnumerator() => this.CaseValues.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.CaseValues).GetEnumerator();

        private RatioLottery<T> Clone()
        {
            var result = new RatioLottery<T>();
            result.cases.AddRange(this.cases);
            result.totalRatio = this.totalRatio;

            return result;
        }

        [DebuggerDisplay("{Value} {Ratio} {AccumulatedRatio}")]
        public readonly struct CaseData
        {
            public CaseData(int ratio, int accumulatedRatio, T value)
            {
                this.Ratio = ratio;
                this.AccumulatedRatio = accumulatedRatio;
                this.Value = value;
            }

            public int Ratio { get; }
            public int AccumulatedRatio { get; }
            public T Value { get; }
        }
    }
}
