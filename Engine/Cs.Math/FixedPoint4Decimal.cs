namespace Cs.Math
{
    using System;

    public readonly record struct FixedPoint4Decimal(long Value)
    {
        private const long ScaleFactor = 10000;

        public FixedPoint4Decimal(double value) : this((long)(value * ScaleFactor))
        {
        }

        public static FixedPoint4Decimal One => new(ScaleFactor);
        public static FixedPoint4Decimal Zero => new(0);

        public double AsDouble => (double)this.Value / ScaleFactor;
        public long Trim => this.Value / ScaleFactor;

        public static FixedPoint4Decimal operator *(FixedPoint4Decimal b, long targetValue) => new(b.Value * targetValue);
        public static FixedPoint4Decimal operator *(FixedPoint4Decimal left, BasisPoint bp) => new(left.Value * bp);
        public static bool operator >(FixedPoint4Decimal left, FixedPoint4Decimal right) => left.Value > right.Value;
        public static bool operator <(FixedPoint4Decimal left, FixedPoint4Decimal right) => left.Value < right.Value;
        public static bool operator >=(FixedPoint4Decimal left, FixedPoint4Decimal right) => left.Value >= right.Value;
        public static bool operator <=(FixedPoint4Decimal left, FixedPoint4Decimal right) => left.Value <= right.Value;
        public static FixedPoint4Decimal operator +(FixedPoint4Decimal left, FixedPoint4Decimal right) => new(left.Value + right.Value);

        public override string ToString() => this.AsDouble.ToString("F4");
    }
}
