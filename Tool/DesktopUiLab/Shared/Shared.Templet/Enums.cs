namespace Shared.Templet;

using System;
using System.ComponentModel;

public static class Enums
{
    public enum L10nType
    {
        [Description("한국어")]
        Korean,
        [Description("영어")]
        English,
        [Description("일본어")]
        Japanese,
        [Description("중국어(간체)")]
        ChineseSimplified,
        [Description("중국어(번체)")]
        ChineseTraditional,
    }

    public static string ToJsonKey(this L10nType self, string prefix)
    {
        var suffix = self switch
        {
            L10nType.Korean => "KOR",
            L10nType.English => "ENG",
            L10nType.Japanese => "JPN",
            L10nType.ChineseSimplified => "CHN", // "CHS",
            L10nType.ChineseTraditional => "CHT",
            _ => throw new ArgumentOutOfRangeException(nameof(self), self, null),
        };

        return $"{prefix}_{suffix}";
    }
}
