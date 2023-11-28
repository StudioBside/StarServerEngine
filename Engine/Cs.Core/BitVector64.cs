namespace Cs.Core
{
    using System;
    using System.Text;

    //// 출처
    //// https://www.codeproject.com/Tips/838362/A-bit-Bit-Field-in-Csharp?msg=4938064
    //// System.Collections.Specialized.BitVector32 의 64bit 버전

    public struct BitVector64 : IEquatable<BitVector64>
    {
        private long data;

        public BitVector64(BitVector64 source)
        {
            this.data = source.data;
        }

        public BitVector64(long source)
        {
            this.data = source;
        }

        public long Data => this.data;
        public long this[BitVector64.Section section]
        {
            get
            {
                return (this.data >> section.Offset) & section.Mask;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Section can't hold negative values");
                }
                else if (value > section.Mask)
                {
                    throw new ArgumentException("Value too large to fit in section");
                }

                this.data &= ~(section.Mask << section.Offset);
                this.data |= value << section.Offset;
            }
        }

        public bool this[long mask]
        {
            get
            {
                return (this.data & mask) == mask;
            }

            set
            {
                if (value)
                {
                    this.data |= mask;
                }
                else
                {
                    this.data &= ~mask;
                }
            }
        }

        public static bool operator ==(BitVector64 left, BitVector64 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BitVector64 left, BitVector64 right)
        {
            return !(left == right);
        }

        // Methods     
        public static long CreateMask()
        {
            return CreateMask(0);   // 1;
        }

        public static long CreateMask(long prev)
        {
            if (prev == 0)
            {
                return 1;
            }
            else if (prev == long.MinValue)
            {
                throw new InvalidOperationException("all bits set");
            }

            return prev << 1;
        }

        public static Section CreateSection(int bit)
        {
            return CreateSection(bit, new Section(0, 0));
        }

        public static Section CreateSection(int bit, BitVector64.Section previous)
        {
            if (bit < 1)
            {
                throw new ArgumentException("bit");
            }

            var mask = (1L << bit) - 1;
            int offset = previous.Offset + NumberOfSetBits(previous.Mask);

            if (offset > 64)
            {
                throw new ArgumentException("Sections cannot exceed 64 bits in total");
            }

            return new Section(mask, (short)offset);
        }

        public static string ToString(BitVector64 value)
        {
            var sb = new StringBuilder(0x2d);
            sb.Append("BitVector64{0x");
            sb.Append(Convert.ToString(value.Data, 16));
            sb.Append("}");

            return sb.ToString();
        }

        public bool Equals(BitVector64 other)
        {
            return this.Data == other.Data;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is BitVector64))
            {
                return false;
            }

            return this.data == ((BitVector64)obj).data;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(this);
        }

        private static int NumberOfSetBits(long i)
        {
            int count = 0;
            for (int bit = 0; bit < 64; bit++)
            {
                if ((i & 0x01) != 0)
                {
                    count++;
                }

                i = i >> 1;
            }

            return count;
        }

        private static int HighestSetBit(long i)
        {
            for (int bit = 63; bit >= 0; bit--)
            {
                long mask = 1L << bit;
                if ((mask & i) != 0)
                {
                    return bit;
                }
            }

            return -1;
        }

        #region Section
        public struct Section
        {
            private readonly long mask;
            private readonly short offset;

            internal Section(long mask, short offset)
            {
                this.mask = mask;
                this.offset = offset;
            }

            public long Mask => this.mask;
            public short Offset => this.offset;

            public static bool operator ==(Section left, Section right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Section left, Section right)
            {
                return !(left == right);
            }

            public static string ToString(Section value)
            {
                var b = new StringBuilder();
                b.Append("Section{0x");
                b.Append(Convert.ToString(value.Mask, 16));
                b.Append(", 0x");
                b.Append(Convert.ToString(value.Offset, 16));
                b.Append("}");

                return b.ToString();
            }

            public bool Equals(Section obj)
            {
                return this.mask == obj.mask &&
                  this.offset == obj.offset;
            }

            public override bool Equals(object? obj)
            {
                if (!(obj is Section))
                {
                    return false;
                }

                return this.Equals((Section)obj);
            }

            public override int GetHashCode()
            {
                return (((short)this.mask).GetHashCode() << 16)
                  + this.offset.GetHashCode();
            }

            public override string ToString()
            {
                return ToString(this);
            }
        }

        #endregion //Section
    }
}
