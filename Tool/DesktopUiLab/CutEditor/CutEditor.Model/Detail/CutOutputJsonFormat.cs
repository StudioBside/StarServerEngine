﻿namespace CutEditor.Model.Detail;

using System.Drawing;
using NKM;
using static CutEditor.Model.Enums;

internal class CutOutputJsonFormat
{
    public long Uid { get; set; }
    public string? ContentsTag { get; set; }
    public string? CutsceneStrId { get; set; }
    public DestAnchorType? JumpAnchorInfo { get; set; }
    public DestAnchorType? RewardAnchor { get; set; }
    public bool WaitClick { get; set; }
    public float WaitTime { get; set; }
    public string[]? BgFadeInStartCol { get; set; }
    public string[]? BgFadeInCol { get; set; }
    public float? BgFadeInTime { get; set; }
    public string[]? BgFadeOutCol { get; set; }
    public float? BgFadeOutTime { get; set; }
    public float? BgFlashBang { get; set; }
    public float? BgCrash { get; set; }
    public float? BgCrashTime { get; set; }
    public string? EndBgmFileName { get; set; }
    public string? BgFileName { get; set; }
    public string? StartBgmFileName { get; set; }
    public string? StartFxSoundName { get; set; }
    public CutsceneClearType? CutsceneClear { get; set; }
    public string? UnitStrId { get; set; }
    public string[]? UnitNameString { get; set; }
    public CameraOffset? CameraOffset { get; set; }
    public Ease? CameraEase { get; set; }
    public CameraOffsetTime? CameraOffsetTime { get; set; }
    public string? UnitMotion { get; set; }
    public bool? UnitQuickSet { get; set; }
    public CutsceneUnitPos? UnitPos { get; set; }
    public EmotionEffect? EmotionEffect { get; set; }
    public UnitEffect? UnitEffect { get; set; }
    public string? UnitTalk_KOR { get; set; }
    public string? UnitTalk_ENG { get; set; }
    public string? UnitTalk_JPN { get; set; }
    public string? UnitTalk_CHNS { get; set; }
    public float? TalkTime { get; set; }
    public TalkPositionControlType? TalkPositionControl { get; set; }
    public ChoiceOutputFormat[]? JumpAnchorData { get; set; }
    public bool? TalkAppend { get; set; }
    public TransitionEffect? TransitionEffect { get; set; }
    public TransitionControl? TransitionControl { get; set; }
    public string? TalkVoice { get; set; }
    public float? BgChangeTime { get; set; }
    public CutsceneAutoHighlight? AutoHighlight { get; set; }
    public CutsceneFilterType? FilterType { get; set; }
    public int? ArcpointId { get; set; }
    public CutsceneSoundLoopControl? StartFxSoundLoopControl { get; set; }
    public CutsceneSoundLoopControl? EndFxLoopControl { get; set; }

    public SlateControlType? SlateControlType { get; set; }
    public int? SlateSectionNo { get; set; }
    public string? AmbientSound { get; set; }
    public bool? NicknameInput { get; set; }
    public ImageNameFadeEffect? ImageNameFadeIn { get; set; }
    public ImageNameFadeEffect? ImageNameFadeOut { get; set; }
    public string? ImageName { get; set; }
    public bool? BgAniScale { get; set; }
    public string[]? BgOffsetScale { get; set; }
    public float? BgOffsetScaleTime { get; set; }
    public bool? BgAniPos { get; set; }
    public string[]? BgPos { get; set; }
    public Ease? BgEase { get; set; }
    public float? BgPosTime { get; set; }

    internal static float? EliminateZero(float source)
    {
        return Math.Abs(source) < 0.0001f
            ? null
            : source;
    }

    internal static string[]? ConvertColor(Color? color)
    {
        if (color is null)
        {
            return null;
        }

        return [
            (color.Value.R / 255f).ToString(),
            (color.Value.G / 255f).ToString(),
            (color.Value.B / 255f).ToString(),
            (color.Value.A / 255f).ToString()
        ];
    }
}
