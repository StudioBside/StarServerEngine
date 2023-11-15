namespace Cs.Core.Util
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1, CharSet = CharSet.Ansi)]
    public readonly struct WeeklyId : IEquatable<WeeklyId>
    {
        [FieldOffset(0)]
        public readonly int Year;

        [FieldOffset(4)]
        public readonly int WeekOfYear;

        [FieldOffset(0)]
        public readonly long Value;

        public WeeklyId(int year, int weekOfYear)
        {
            this.Value = 0;
            this.Year = year;
            this.WeekOfYear = weekOfYear;
        }

        public static WeeklyId Invalid => new WeeklyId(year: 0, weekOfYear: 0);

        public static bool operator ==(WeeklyId left, WeeklyId right) => left.Equals(right);
        public static bool operator !=(WeeklyId left, WeeklyId right) => !(left == right);

        public override bool Equals(object? other)
        {
            // null 확인
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            // struct는 value type이어서 참조 동일성 확인 안함.
            if (other is WeeklyId typed)
            {
                return this.Equals(typed);
            }

            return false;
        }

        public bool Equals(WeeklyId other) => this.Value == other.Value;
        public override int GetHashCode() => this.Value.GetHashCode();

        public override string ToString() => $"{this.Year}/{this.WeekOfYear.DisplayWithSuffix()}";
    }
}
