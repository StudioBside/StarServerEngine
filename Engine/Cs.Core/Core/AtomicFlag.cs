namespace Cs.Core.Core
{
    using System.Diagnostics;
    using System.Threading;

    [DebuggerDisplay("value = {value}")]
    public sealed class AtomicFlag
    {
        private volatile int value = 1; // atomic 연산을 위해 int를 사용. 0과 1만 쓰인다. 

        public AtomicFlag(bool initialValue)
        {
            this.value = initialValue ? 1 : 0;
        }

        public bool IsOn => this.value == 1;

        public bool On()
        {
            int prevValue = Interlocked.CompareExchange(ref this.value, 1, 0); // 0 -> 1
            return prevValue == 0;
        }

        public bool Off()
        {
            int prevValue = Interlocked.CompareExchange(ref this.value, 0, 1); // 1 -> 0
            return prevValue == 1;
        }
    }
}
