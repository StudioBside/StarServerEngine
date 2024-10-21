namespace Cs.Math
{
    using System;

    public readonly record struct BasisPoint(int Value)
    {
        public static BasisPoint Max => new(10000);
        public static BasisPoint Zero => new(0);

        public static int operator *(int targetValue, BasisPoint b) => targetValue * b.Value / 10000;
        public static int operator *(BasisPoint b, int targetValue) => targetValue * b.Value / 10000;
        public static long operator *(long targetValue, BasisPoint b) => targetValue * b.Value / 10000;
        public static long operator *(BasisPoint b, long targetValue) => targetValue * b.Value / 10000;
        public static bool operator >(BasisPoint left, BasisPoint right) => left.Value > right.Value;
        public static bool operator <(BasisPoint left, BasisPoint right) => left.Value < right.Value;
        public static bool operator >=(BasisPoint left, BasisPoint right) => left.Value >= right.Value;
        public static bool operator <=(BasisPoint left, BasisPoint right) => left.Value <= right.Value;
        public static BasisPoint operator +(BasisPoint left, BasisPoint right) => new BasisPoint(left.Value + right.Value);

        public override string ToString() => $"{this.Value} bp ({this.Value / 100.0:0.##}%)";
        public string ToPercentString() => $"{this.Value / 100.0:0.##}%";
    }
}
