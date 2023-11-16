namespace Cs.Core.Core
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    public sealed class FixedMPSCQueue<T> where T : class
    {
        private readonly CircularBuffer<T> buffer;
        private int head = 0;
        private int tail = 0;

        public FixedMPSCQueue(int exponent)
        {
            this.buffer = new CircularBuffer<T>(exponent);
        }

        public int MaxSize => this.buffer.MaxSize;
        public int Count => this.head - this.tail;
        public int ReservedSize => this.MaxSize - this.Count;

        public void Enqueue(T item)
        {
            Debug.Assert(this.ReservedSize > 0, $"[FixedMPSCQueue] buffer full. count:{this.Count}");

            int position = Interlocked.Increment(ref this.head) - 1;
            this.buffer[position] = item;
        }

        [return: MaybeNull]
        public T TryDequeue()
        {
            if (this.Count <= 0)
            {
                return default;
            }

            int position = Interlocked.Increment(ref this.tail) - 1;
            Debug.Assert(this.tail <= this.head, $"[FixedMPSCQueue] logical error");

            var result = this.buffer[position];
            this.buffer[position] = default!;
            return result;
        }

        public bool TryDequeue([MaybeNullWhen(false)] out T result)
        {
            if (this.Count <= 0)
            {
                result = default;
                return false;
            }

            int position = Interlocked.Increment(ref this.tail) - 1;
            Debug.Assert(this.tail <= this.head, $"[FixedMPSCQueue] logical error");

            result = this.buffer[position];
            this.buffer[position] = default!;
            return true;
        }
    }
}
