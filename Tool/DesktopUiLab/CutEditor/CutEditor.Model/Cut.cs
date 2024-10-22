namespace CutEditor.Model;

using System;
using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Templet.Base;
using Shared.Templet.TempletTypes;

public sealed class Cut : ObservableObject
{
    private readonly L10nText unitTalk = new();
    private string? contentsTag;
    private string? cutsceneStrId;
    private bool waitClick;
    private float waitTime;
    private string? bgFadeInStartCol;
    private string? bgFadeInCol;
    private float bgFadeInTime;
    private string? bgFadeOutCol;
    private float bgFadeOutTime;
    private float bgFlashBang;
    private float bgCrash;
    private float bgCrashTime;
    private string? endBgmFileName;
    private string? cutsceneClear; // enum
    private string? bgFileName;
    private string? startBgmFileName;
    private string? startFxSoundName;
    private string? unitName;
    private string? emotionEffect; // enum
    private string? unitStrId;
    private Unit? unit;
    private bool unitQuickSet;
    private string? unitPos; // enum
    private string? cameraOffset; // enum
    private string? cameraOffsetTime; // enum
    private float talkTime;
    private string? talkPositionControl; // enum

    public Cut(JToken token, long uid) : this(uid)
    {
        this.contentsTag = token.GetString("ContentsTag", null!);
        this.cutsceneStrId = token.GetString("CutsceneStrId", null!);
        this.waitClick = token.GetBool("WaitClick", false);
        this.waitTime = token.GetFloat("WaitTime", 0f);
        this.bgFadeInStartCol = token.GetString("BgFadeInStartCol", null!);
        this.bgFadeInCol = token.GetString("BgFadeInCol", null!);
        this.bgFadeInTime = token.GetFloat("BgFadeInTime", 0f);
        this.bgFadeOutCol = token.GetString("BgFadeOutCol", null!);
        this.bgFadeOutTime = token.GetFloat("BgFadeOutTime", 0f);
        this.bgFlashBang = token.GetFloat("BgFlashBang", 0f);
        this.bgCrash = token.GetFloat("BgCrash", 0f);
        this.bgCrashTime = token.GetFloat("BgCrashTime", 0f);
        this.endBgmFileName = token.GetString("EndBgmFileName", null!);
        this.cutsceneClear = token.GetString("CutsceneClear", null!);
        this.bgFileName = token.GetString("BgFileName", null!);
        this.startBgmFileName = token.GetString("StartBgmFileName", null!);
        this.startFxSoundName = token.GetString("StartFxSoundName", null!);
        this.unitQuickSet = token.GetBool("UnitQuickSet", false);
        this.unitPos = token.GetString("UnitPos", null!);
        this.cameraOffset = token.GetString("CameraOffset", null!);
        this.cameraOffsetTime = token.GetString("CameraOffsetTime", null!);

        this.unitName = token.GetString("UnitNameString", null!);
        this.emotionEffect = token.GetString("EmotionEffect", null!);
        this.unitTalk.Load(token, "UnitTalk");
        this.talkTime = token.GetFloat("TalkTime", 0f);
        this.unitStrId = token.GetString("UnitStrId", null!);
        this.talkPositionControl = token.GetString("TalkPositionControl", null!);

        if (string.IsNullOrEmpty(this.unitStrId) == false)
        {
            this.unit = TempletContainer<Unit>.Find(this.unitStrId);
            if (this.unit is null)
            {
                Log.Error($"유닛 템플릿을 찾을 수 없습니다. UnitStrId:{this.unitStrId}");
            }
        }
    }

    public Cut(long uid)
    {
        this.Uid = uid;
    }

    public long Uid { get; }
    public L10nText UnitTalk => this.unitTalk;
    public string? UnitName
    {
        get => this.unitName;
        set => this.SetProperty(ref this.unitName, value);
    }

    public float TalkTime
    {
        get => this.talkTime;
        set => this.SetProperty(ref this.talkTime, value);
    }

    public Unit? Unit
    {
        get => this.unit;
        set => this.SetProperty(ref this.unit, value);
    }

    public object ToOutputType()
    {
        return new
        {
            Uid = this.Uid,
            ContentsTag = this.contentsTag,
            CutsceneStrId = this.cutsceneStrId,
            WaitClick = this.waitClick,
            WaitTime = this.waitTime,
            BgFadeInStartCol = this.bgFadeInStartCol,
            BgFadeInCol = this.bgFadeInCol,
            BgFadeInTime = EliminateZero(this.bgFadeInTime),
            BgFadeOutCol = this.bgFadeOutCol,
            BgFadeOutTime = EliminateZero(this.bgFadeOutTime),
            BgFlashBang = EliminateZero(this.bgFlashBang),
            BgCrash = EliminateZero(this.bgCrash),
            BgCrashTime = EliminateZero(this.bgCrashTime),
            EndBgmFileName = this.endBgmFileName,
            BgFileName = this.bgFileName,
            StartBgmFileName = this.startBgmFileName,
            StartFxSoundName = this.startFxSoundName,
            CutsceneClear = this.cutsceneClear,
            UnitStrId = this.unitStrId,
            UnitNameString = this.unitName,
            UnitQuickSet = EliminateFalse(this.unitQuickSet),
            UnitPos = this.unitPos,
            CameraOffset = this.cameraOffset,
            CameraOffsetTime = this.cameraOffsetTime,
            EmotionEffect = this.emotionEffect,
            UnitTalk_KOR = EliminateEmpty(this.unitTalk.Korean),
            UnitTalk_ENG = EliminateEmpty(this.unitTalk.English),
            UnitTalk_JPN = EliminateEmpty(this.unitTalk.Japanese),
            UnitTalk_CHN = EliminateEmpty(this.unitTalk.ChineseSimplified),
            //UnitTalk_CHT = EliminateEmpty(this.unitTalk.ChineseTraditional),
            TalkTime = EliminateZero(this.talkTime),
            TalkPositionControl = this.talkPositionControl,
        };

        static string? EliminateEmpty(string source)
        {
            return string.IsNullOrEmpty(source)
                ? null
                : source;
        }

        static float? EliminateZero(float source)
        {
            return Math.Abs(source) < 0.0001f
                ? null
                : source;
        }

        static bool? EliminateFalse(bool source)
        {
            return source ? source : null;
        }
    }

    //// --------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.Unit):
                this.ResetUnitStrId();
                break;
        }
    }

    private void ResetUnitStrId()
    {
        if (this.unit is null)
        {
            this.unitStrId = string.Empty;
            return;
        }

        this.unitStrId = this.unit.StrId;
    }
}
