namespace CutEditor.Model.Detail;

using NKM;

internal class CutOutputFormat
{
    public long Uid { get; set; }
    public string? ContentsTag { get; set; }
    public string? CutsceneStrId { get; set; }
    public bool WaitClick { get; set; }
    public float WaitTime { get; set; }
    public int[]? BgFadeInStartCol { get; set; }
    public int[]? BgFadeInCol { get; set; }
    public float? BgFadeInTime { get; set; }
    public int[]? BgFadeOutCol { get; set; }
    public float? BgFadeOutTime { get; set; }
    public float? BgFlashBang { get; set; }
    public float? BgCrash { get; set; }
    public float? BgCrashTime { get; set; }
    public string? EndBgmFileName { get; set; }
    public string? BgFileName { get; set; }
    public string? StartBgmFileName { get; set; }
    public string? StartFxSoundName { get; set; }
    public string? CutsceneClear { get; set; }
    public string? UnitStrId { get; set; }
    public string[]? UnitNameString { get; set; }
    public bool? UnitQuickSet { get; set; }
    public CutsceneUnitPos? UnitPos { get; set; }
    public string? CameraOffset { get; set; }
    public string? CameraOffsetTime { get; set; }
    public string? EmotionEffect { get; set; }
    public string? UnitTalk_KOR { get; set; }
    public string? UnitTalk_ENG { get; set; }
    public string? UnitTalk_JPN { get; set; }
    public string? UnitTalk_CHN { get; set; }
    public float? TalkTime { get; set; }
    public int[]? TalkPositionControl { get; set; }
    public object[]? JumpAnchorData { get; set; }
    public bool? TalkAppend { get; set; }
}