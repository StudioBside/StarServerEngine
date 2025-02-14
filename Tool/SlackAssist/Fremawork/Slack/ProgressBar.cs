namespace SlackAssist.Fremawork.Slack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal static class ProgressBar
{
    private const string Block = "█";
    private const string Blank = " ⁢";
    private const int DefaultPorgressLength = 20;

    /// <summary>
    /// collection으로 받은 값들 중에, predicate 조건을 만족하는 값들의 개수를 기준으로 진행률을 표시합니다.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
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
        float rate = Math.Clamp((float)value / total, 0.0f, 1.0f);
        float percent = rate * 100f;

        int blockCount = (int)MathF.Round(rate * progressLength);

        StringBuilder sb = new();
        sb.Insert(0, Block, blockCount);
        sb.Insert(blockCount, Blank, progressLength - blockCount);

        return $"*`{sb}`* {value} / {total} ({percent:0.#}%)";
    }

    /// <summary>
    /// 채워져야 할 양을 백분율로 받고, 프로그레스바의 길이를 받아 bar를 만듭니다. 추가 텍스트는 붙지 않습니다.
    /// </summary>
    /// <param name="percent">백분율.</param>
    /// <param name="progressLength">프로그레스바의 길이.</param>
    public static string CreateSimple(int percent, int progressLength)
    {
        int blockCount = (int)MathF.Round(percent * 0.01f * progressLength);

        StringBuilder sb = new();
        sb.Insert(0, Block, blockCount);
        sb.Insert(blockCount, Blank, progressLength - blockCount);

        return $"*`{sb}`*";
    }
}