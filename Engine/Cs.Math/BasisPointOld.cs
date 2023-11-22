namespace Cs.Math
{
    using System;

    // 클라이언트와 같이 쓰기 위해 C# 8.0 이전 문법으로 작성한 코드
    public readonly struct BasisPointOld : IEquatable<BasisPointOld>
    {
        public BasisPointOld(int value)
        {
            this.Value = value;
        }

        public static BasisPointOld Max => new BasisPointOld(10000);
        public static BasisPointOld Zero => new BasisPointOld(0);
        public int Value { get; }
        public bool HasValue => this.Value > 0;

        public static bool operator ==(BasisPointOld left, BasisPointOld right) => left.Value == right.Value;
        public static bool operator !=(BasisPointOld left, BasisPointOld right) => left.Value != right.Value;
        public static int operator *(int targetValue, BasisPointOld b) => targetValue * b.Value / 10000;
        public static int operator *(BasisPointOld b, int targetValue) => targetValue * b.Value / 10000;
        public static long operator *(long targetValue, BasisPointOld b) => targetValue * b.Value / 10000;
        public static long operator *(BasisPointOld b, long targetValue) => targetValue * b.Value / 10000;
        public static bool operator >(BasisPointOld left, BasisPointOld right) => left.Value > right.Value;
        public static bool operator <(BasisPointOld left, BasisPointOld right) => left.Value < right.Value;
        public static bool operator >=(BasisPointOld left, BasisPointOld right) => left.Value >= right.Value;
        public static bool operator <=(BasisPointOld left, BasisPointOld right) => left.Value <= right.Value;

        public override string ToString()
        {
            return $"{this.Value} bp ({this.Value / 100.0:0.##}%)";
        }

        public bool HasValidRange()
        {
            return this.Value >= 0 && this.Value <= 10000;
        }

        public override bool Equals(object obj)
        {
            return obj is BasisPointOld old && this.Equals(old);
        }

        public bool Equals(BasisPointOld other)
        {
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value);
        }

        public double ToDouble()
        {
            return this.Value / 10000.0;
        }

        public string ToPercentString()
        {
            return $"{this.Value / 100.0:0.##}%";
        }
    }
}
