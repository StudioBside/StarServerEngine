namespace StringStorage;

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
        [Description("중국어(간체)")]
        ChineseSimplified,
        [Description("중국어(번체)")]
        ChineseTraditional,
        [Description("일본어")]
        Japanese,
    }
}
