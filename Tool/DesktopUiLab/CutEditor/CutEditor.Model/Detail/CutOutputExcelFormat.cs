﻿namespace CutEditor.Model.Detail;

using NKM;
using static CutEditor.Model.Enums;

public sealed class CutOutputExcelFormat
{
    public CutOutputExcelFormat(Cut cut)
    {
        this.Uid = cut.Uid.ToString();
        this.StartAnchor = null;
        this.DestAnchor = OutputTransfer.EliminateEnum(cut.JumpAnchor, DestAnchorType.None);

        this.Speaker = cut.SpeakerName;
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

    public string Uid { get; }
    public DestAnchorType? DestAnchor { get; } // dest가 엑셀에서 먼저 나오는게 더 나아 보여서 앞으로 이동.
    public StartAnchorType? StartAnchor { get; }
    public string? Speaker { get; }
    public string? UnitMotion { get; }
    public EmotionEffect? Emotion { get; }
    public string Korean { get; }
    public string English { get; }
    public string Japanese { get; }
    public string ChineseSimplified { get; }
}
