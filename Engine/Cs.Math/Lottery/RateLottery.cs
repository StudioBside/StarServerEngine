namespace Cs.Math.Lottery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class RateLottery<T>
    {
        private const int MaxRate = 10000;

        private readonly List<CaseData> cases = new();
        private readonly T defaultValue;
        private int totalRate;

        public RateLottery(T defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public int TotalRate => this.totalRate;
        public int CaseCount => this.cases.Count;
        public bool HasDefaultRate => this.totalRate < MaxRate;
        public bool HasFullRate => this.totalRate == MaxRate;
        public IEnumerable<CaseData> Cases => this.HasFullRate ? this.cases : this.cases.Union(new CaseData[] { this.DefaultCase });
        private CaseData DefaultCase => new CaseData(MaxRate - this.totalRate, MaxRate, this.defaultValue);

        public void AddCase(int rate, T value)
        {
            if (rate < 0 || this.totalRate + rate > MaxRate)
            {
                throw new Exception($"rate value overflow. current:{this.totalRate} add:{rate}");
            }

            if (rate == 0)
            {
                // 확률이 없는 케이스는 아예 받지 않는다.
                return;
            }

            this.totalRate += rate;
            this.cases.Add(new CaseData(rate, this.totalRate, value));
        }

        public bool Decide(out T result)
        {
            int randomValue = RandomGenerator.Next(MaxRate);
            foreach (CaseData data in this.cases)
            {
                if (randomValue < data.AccumulatedRate)
                {
                    result = data.Value;
                    return true;
                }
            }

            result = this.defaultValue;
            return false;
        }

        public T Decide()
        {
            this.Decide(out T result);
            return result;
        }

        public bool Decide(out CaseData result)
        {
            int randomValue = RandomGenerator.Next(MaxRate);
            foreach (CaseData data in this.cases)
            {
                if (randomValue < data.AccumulatedRate)
                {
                    result = data;
                    return true;
                }
            }

            result = this.DefaultCase;
            return false;
        }

        public CaseData DecideDetail()
        {
            this.Decide(out CaseData result);
            return result;
        }

        public CaseData GetCaseByValue(Predicate<T> predicate)
        {
            // note: default case도 포함하도록 변수 대신 속성 사용
            foreach (var data in this.Cases)
            {
                if (predicate(data.Value))
                {
                    return data;
                }
            }

            throw new InvalidOperationException($"[RateLottery] required case not found. caseCount:{this.cases.Count}");
        }

        public bool TryGetRatePercent(T value, out float ratePercent)
        {
            // note: default case도 포함하도록 변수 대신 속성 사용
            foreach (var data in this.Cases)
            {
                if (data.Value.Equals(value))
                {
                    ratePercent = data.RatePercent;
                    return true;
                }
            }

            ratePercent = default;
            return false;
        }

        public bool HasValue(T value) => this.cases.Any(e => e.Value.Equals(value));

        public readonly struct CaseData
        {
            public CaseData(int rate, int accumulatedRate, T value)
            {
                this.Rate = rate;
                this.AccumulatedRate = accumulatedRate;
                this.Value = value;
            }

            public int Rate { get; }
            public int AccumulatedRate { get; }
            public T Value { get; }
            public float RatePercent => this.Rate * 0.01f;

            public override string ToString() => $"{this.Value.ToString()} ({this.RatePercent:0.00}%)";
        }
    }
}
