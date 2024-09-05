namespace Cs.Core.Core
{
    using System.Collections.Generic;

    public interface ICircularBuffer
    {
        int ToSafeIndex(long index);
    }

    public sealed class CircularBuffer<T> : ICircularBuffer
    {
        private readonly long kMask = 0;
        private T[] buffer;

        public CircularBuffer(int exponent)
        {
            int maxSize = 1 << exponent;
            this.kMask = maxSize - 1;
            this.buffer = new T[maxSize];
        }

        public int MaxSize => this.buffer.Length;
        public IReadOnlyList<T> RawBuffer => this.buffer;

        public T this[long index]
        {
            get => this.buffer[index & this.kMask];
            set => this.buffer[index & this.kMask] = value;
        }

        public int ToSafeIndex(long index) => (int)(index & this.kMask);

        public void Regulate(long decrement)
        {
            // note: not thread-safe.
            var clone = new T[this.MaxSize];

            for (int i = 0; i < clone.Length; ++i)
            {
                clone[i] = this[i + decrement];
            }

            this.buffer = clone;
        }

        public T[] ToArray(long startIndex)
        {
            // note: not thread-safe.
            var result = new T[this.MaxSize];

            for (long i = 0; i < this.MaxSize; ++i)
            {
                result[i] = this[i + startIndex];
            }

            return result;
        }
    }
}
