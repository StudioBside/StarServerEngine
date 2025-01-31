namespace Du.Core.Util;
using System;
using Cs.Core.Core;

public readonly struct ReEnterGuard : IDisposable
{
    private readonly AtomicFlag flag;

    private ReEnterGuard(AtomicFlag flag)
    {
        this.flag = flag;
    }

    public bool IsValid => this.flag != null;

    public static bool TryEnter(AtomicFlag flag, out ReEnterGuard guard)
    {
        if (flag.On() == false)
        {
            guard = default;
            return false;
        }

        guard = new(flag);
        return true;
    }

    public static ReEnterGuard? TryEnter(AtomicFlag flag)
    {
        if (flag.On() == false)
        {
            return null;
        }

        return new(flag);
    }

    public void Dispose()
    {
        this.flag.Off();
    }
}
