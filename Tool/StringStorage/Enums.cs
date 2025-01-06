namespace StringStorage;

using System;
using System.ComponentModel;

public static class Enums
{
    public enum L10nType
    {
        [Description("한국어")]
        Kor,
        [Description("영어")]
        Eng,
        [Description("중국어(간체)")]
        ChnS,
        [Description("중국어(번체)")]
        ChnT,
        [Description("일본어")]
        Jpn,
    }
}
