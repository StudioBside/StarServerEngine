namespace Cs.Core.Util;

using System.Collections;
using System.Diagnostics;

public static class BitArrayExt
{
    public static byte[] ToByteArray(this BitArray data)
    {
        Debug.Assert(data.Length % 8 == 0, $"invalid length:{data.Length}"); // 바이트 단위로 나누어 떨어져야 함.

        int byte_count = data.Length / 8;
        byte[] buffer = new byte[byte_count];
        data.CopyTo(buffer, 0);

        return buffer;
    }

    public static int GetByteCount(this BitArray data)
    {
        Debug.Assert(data.Length % 8 == 0, $"invalid length:{data.Length}"); // 바이트 단위로 나누어 떨어져야 함.
        return data.Length / 8;
    }
}
