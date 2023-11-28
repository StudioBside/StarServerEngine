namespace Cs.Math.Lottery
{
    using System;
    using System.Collections.Generic;

    public readonly ref struct ValueRatioLottery<T>
    {
        private readonly CaseData[] cases;
        private readonly int totalRatio;

        public ValueRatioLottery(IReadOnlyList<(int Ratio, T Value)> list)
        {
            this.cases = new CaseData[list.Count];
            this.totalRatio = 0;

            int index = 0;
            foreach (var pair in list)
            {
                if (pair.Ratio < 0)
                {
                    throw new Exception($"invalid ratio data. ratio:{pair.Ratio}");
                }

                this.totalRatio += pair.Ratio;
                this.cases[index] = new CaseData(pair.Ratio, this.totalRatio, pair.Value);

                ++index;
            }
        }

        public Result Decide()
        {
            int randomValue = RandomGenerator.Next(this.totalRatio);
            foreach (CaseData data in this.cases)
            {
                if (randomValue < data.AccumulatedRatio)
                {
                    float ratePercent = data.Ratio * 100f / this.totalRatio;
                    return new Result(ratePercent, data.Value);
                }
            }

            throw new Exception($"[RatioLottery] pick failed. randomValue:{randomValue} totalRatio:{this.totalRatio}");
        }

        public readonly struct Result
        {
            public Result(float ratePercent, T value)
            {
                this.RatePercent = ratePercent;
                this.Value = value;
            }

            public float RatePercent { get; }
            public T Value { get; }

            public void Deconstruct(out float ratePercent, out T result)
            {
                ratePercent = this.RatePercent;
                result = this.Value;
            }

            public override string ToString() => $"{this.Value.ToString()} ({this.RatePercent:0.00}%)";
        }

        private readonly struct CaseData
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
