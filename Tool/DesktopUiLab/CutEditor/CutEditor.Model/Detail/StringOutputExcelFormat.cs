﻿namespace CutEditor.Model.Detail;

using Shared.Templet.Strings;

public sealed class StringOutputExcelFormat
{
    public StringOutputExcelFormat(StringElement element)
    {
        this.PrimeKey = element.PrimeKey;
        this.Korean = element.Korean;
        this.English = element.English;
        this.Japanese = element.Japanese;
        this.ChineseSimplified = element.ChineseSimplified;
    }

    public StringOutputExcelFormat()
    {
        // note: file에서 읽어들일 때 사용하기 때문에 빈 생성자를 만들어둡니다.
    }

    public string PrimeKey { get; set; } = string.Empty;
    public string Korean { get; set; } = string.Empty;
    public string English { get; set; } = string.Empty;
    public string Japanese { get; set; } = string.Empty;
    public string ChineseSimplified { get; set; } = string.Empty;
}
