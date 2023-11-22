namespace Cs.Math.Lottery
{
    using System;
    using System.Collections.Generic;
    using Cs.Logging;

    public readonly struct DependentLottery<T>
    {
        private const int MaxRate = 10000;
        private readonly T defaultValue;
        private readonly List<CaseData> cases = new();

        public DependentLottery(T defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public void AddCase(int rate, T value)
        {
            if (rate < 0)
            {
                throw new Exception($"rate value overflow. current:{rate}");
            }

            if (rate > MaxRate)
            {
                Log.Warn($"rate value overflow. Rate:{rate}");
                rate = MaxRate;
            }

            this.cases.Add(new CaseData(rate, value));
        }

        public T Decide()
        {
            this.Decide(out T result);
            return result;
        }

        public bool Decide(out T result)
        {
            foreach (CaseData data in this.cases)
            {
                int randomValue = RandomGenerator.Next(MaxRate);

                if (randomValue < data.Rate)
                {
                    result = data.Value;
                    return true;
                }
            }

            result = this.defaultValue;
            return false;
        }

        private readonly struct CaseData
        {
            public CaseData(int rate, T value)
            {
                this.Value = value;
                this.Rate = rate;
            }

            public int Rate { get; }
            public T Value { get; }
        }
    }
}
