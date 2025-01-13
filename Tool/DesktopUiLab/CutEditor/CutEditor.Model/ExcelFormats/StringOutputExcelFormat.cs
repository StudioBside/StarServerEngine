namespace CutEditor.Model.ExcelFormats;

using Shared.Interfaces;
using Shared.Templet.Strings;
using static StringStorage.Enums;

public sealed class StringOutputExcelFormat : IL10nText
{
    public StringOutputExcelFormat(StringElement element)
    {
        this.PrimeKey = element.PrimeKey;
        this.Korean = element.Korean;
        this.English = element.English;
        this.Japanese = element.Japanese;
        this.ChineseSimplified = element.ChineseSimplified;
        this.ChineseTraditional = element.ChineseTraditional;
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
    public string ChineseTraditional { get; set; } = string.Empty;

    public string Get(L10nType l10nType)
    {
        return l10nType switch
        {
            L10nType.Kor => this.Korean,
            L10nType.Eng => this.English,
            L10nType.Jpn => this.Japanese,
            L10nType.ChnS => this.ChineseSimplified,
            L10nType.ChnT => this.ChineseTraditional,
            _ => throw new ArgumentOutOfRangeException(nameof(l10nType), l10nType, message: null),
        };
    }
}
