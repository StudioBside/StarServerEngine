﻿namespace CutEditor.Model.Detail;

using NKM;
using Shared.Interfaces;
using static CutEditor.Model.Enums;
using static StringStorage.Enums;

public sealed class CutOutputExcelFormat : IL10nText
{
    public CutOutputExcelFormat(Cut cut)
    {
        this.Uid = cut.Uid.ToString();
        this.StartAnchor = null;
        this.DestAnchor = OutputTransfer.EliminateEnum(cut.JumpAnchor, DestAnchorType.None);

        this.SpeakerKor = cut.SpeakerNameKor;
        this.SpeakerJpn = cut.SpeakerNameJpn;
        this.UnitMotion = cut.UnitMotion;
        this.Emotion = OutputTransfer.EliminateEnum(cut.EmotionEffect, EmotionEffect.NONE);
        this.Korean = cut.UnitTalk.Korean;
        this.English = cut.UnitTalk.English;
        this.Japanese = cut.UnitTalk.Japanese;
        this.ChineseSimplified = cut.UnitTalk.ChineseSimplified;
    }

    public CutOutputExcelFormat(ChoiceOption choice)
    {
        this.Uid = choice.UidString;
        this.StartAnchor = OutputTransfer.EliminateEnum(choice.JumpAnchor, StartAnchorType.None);
        this.DestAnchor = null;

        this.Korean = choice.Text.Korean;
        this.English = choice.Text.English;
        this.Japanese = choice.Text.Japanese;
        this.ChineseSimplified = choice.Text.ChineseSimplified;
    }

    public CutOutputExcelFormat()
    {
        // note: file에서 읽어들일 때 사용하기 때문에 빈 생성자를 만들어둡니다.
    }

    public string Uid { get; set; } = string.Empty;
    public DestAnchorType? DestAnchor { get; set; } // dest가 엑셀에서 먼저 나오는게 더 나아 보여서 앞으로 이동.
    public StartAnchorType? StartAnchor { get; set; }
    public string? SpeakerKor { get; set; }
    public string? SpeakerJpn { get; set; }
    public string? UnitMotion { get; set; }
    public EmotionEffect? Emotion { get; set; }
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
            _ => string.Empty,
        };
    }
}
