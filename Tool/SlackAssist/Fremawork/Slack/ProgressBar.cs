namespace SlackAssist.Fremawork.Slack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal static class ProgressBar
{
    private const string Block = "█";
    private const string Blank = " ⁢";
    private const int DefaultPorgressLength = 20;

    public static string ToProgressBar<T>(this IReadOnlyList<T> collection, Func<T, bool> predicate)
    {
        int value = collection.Count(predicate);
        return Create(value, collection.Count);
    }

    public static string Create(int value, int total)
    {
        return Create(value, total, DefaultPorgressLength);
    }

    public static string Create(int value, int total, int progressLength)
    {
        if (progressLength <= 0)
        {
            progressLength = DefaultPorgressLength;
        }

        float rate = Math.Clamp((float)value / total, 0.0f, 1.0f);
        float percent = rate * 100f;

        int blockCount = (int)MathF.Round(rate * progressLength);

        StringBuilder sb = new();
        sb.Insert(0, Block, blockCount);
        sb.Insert(blockCount, Blank, progressLength - blockCount);

        return $"*`{sb}`* {value} / {total} ({percent:0.#}%)";
    }
}