namespace CutEditor.Model;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model.Detail;
using Newtonsoft.Json.Linq;
using NKM;
using Shared.Templet.Base;
using Shared.Templet.TempletTypes;
using static CutEditor.Model.Enums;
using static Shared.Templet.Enums;

public sealed class Cut : ObservableObject
{
    private readonly L10nText unitTalk = new();
    private readonly ObservableCollection<ChoiceOption> choices = new();
    private readonly ObservableCollection<string> unitNames = new();
    private string? contentsTag;
    private string? cutsceneStrId;
    private bool waitClick = true;
    private float waitTime;
    private BgFadeInOut? bgFadeInOut;
    private float bgFlashBang;
    private float bgCrash;
    private float bgCrashTime;
    private string? endBgmFileName;
    private string? endFxSoundName;
    private CutsceneClearType cutsceneClear;
    private string? bgFileName;
    private string? startBgmFileName;
    private string? startFxSoundName;
    private EmotionEffect emotionEffect;
    private string? unitStrId; // 로딩할 때만 쓰고, 변경이 있을 땐 쓰지 않음. 
    private Unit? unit;
    private bool unitQuickSet;
    private CutsceneUnitPos unitPos;
    private string? cameraOffset; // enum
    private string? cameraOffsetTime; // enum
    private float talkTime;
    private TalkPositionControlType talkPositionControl;
    private bool talkAppend;
    private DestAnchorType jumpAnchor;
    private string? unitMotion;
    private TransitionEffect? transitionEffect;
    private TransitionControl? transitionControl;
    private string? talkVoice;
    private float bgChangeTime;
    private CutsceneAutoHighlight autoHighlight;
    private CutsceneFilterType filterType;
    private int arcpointId; // 로딩할 때만 쓰고, 변경이 있을 땐 쓰지 않음.
    private LobbyItem? arcpoint;
    private CutsceneSoundLoopControl startFxLoopControl;
    private CutsceneSoundLoopControl endFxLoopControl;
    private SlateControlType slateControlType;
    private int slateSectionNo;
    private string? ambientSound;

    public Cut(long uid)
    {
        this.Uid = uid;
    }

    public Cut(JToken token)
    {
        this.Uid = token.GetInt64("Uid", 0);
        this.contentsTag = token.GetString("ContentsTag", null!);
        this.cutsceneStrId = token.GetString("CutsceneStrId", null!);
        this.waitClick = token.GetBool("WaitClick", true);
        this.waitTime = token.GetFloat("WaitTime", 0f);
        this.bgFadeInOut = BgFadeInOut.Create(token);
        this.bgFlashBang = token.GetFloat("BgFlashBang", 0f);
        this.bgCrash = token.GetFloat("BgCrash", 0f);
        this.bgCrashTime = token.GetFloat("BgCrashTime", 0f);
        this.endBgmFileName = token.GetString("EndBgmFileName", null!);
        this.endFxSoundName = token.GetString("EndFxSoundName", null!);
        this.cutsceneClear = token.GetEnum("CutsceneClear", CutsceneClearType.NONE);
        this.bgFileName = token.GetString("BgFileName", null!);
        this.startBgmFileName = token.GetString("StartBgmFileName", null!);
        this.startFxSoundName = token.GetString("StartFxSoundName", null!);
        this.unitQuickSet = token.GetBool("UnitQuickSet", false);
        this.unitPos = token.GetEnum("UnitPos", CutsceneUnitPos.NONE);
        this.cameraOffset = token.GetString("CameraOffset", null!);
        this.cameraOffsetTime = token.GetString("CameraOffsetTime", null!);

        this.emotionEffect = token.GetEnum("EmotionEffect", EmotionEffect.NONE);
        this.unitTalk.Load(token, "UnitTalk");
        this.talkTime = token.GetFloat("TalkTime", 0f);
        this.unitStrId = token.GetString("UnitStrId", null!);
        this.talkPositionControl = token.GetEnum("TalkPositionControl", TalkPositionControlType.NONE);
        token.TryGetArray("JumpAnchorData", this.choices, ChoiceOption.Load);
        token.TryGetArray("UnitNameString", this.unitNames);
        this.talkAppend = token.GetBool("TalkAppend", false);
        this.unitMotion = token.GetString("UnitMotion", null!);
        this.talkVoice = token.GetString("TalkVoice", null!);
        this.bgChangeTime = token.GetFloat("BgChangeTime", 0f);
        this.autoHighlight = token.GetEnum("AutoHighlight", CutsceneAutoHighlight.NONE);
        this.filterType = token.GetEnum("FilterType", CutsceneFilterType.NONE);
        this.arcpointId = token.GetInt32("ArcpointId", 0);
        this.startFxLoopControl = token.GetEnum("StartFxLoopControl", CutsceneSoundLoopControl.NONE);
        this.endFxLoopControl = token.GetEnum("EndFxLoopControl", CutsceneSoundLoopControl.NONE);
        this.slateControlType = token.GetEnum("SlateControlType", SlateControlType.NONE);
        this.slateSectionNo = token.GetInt32("SlateSectionNo", 0);
        this.ambientSound = token.GetString("AmbientSound", null!);
        if (token.TryGetEnum<TransitionEffect>("TransitionEffect", out var transitionEffect))
        {
            this.transitionEffect = transitionEffect;
        }

        if (token.TryGetEnum<TransitionControl>("TransitionControl", out var transitionControl))
        {
            this.transitionControl = transitionControl;
        }

        if (token.TryGetString("JumpAnchorInfo", out var anchorStr))
        {
            this.jumpAnchor = Enum.Parse<DestAnchorType>(anchorStr);
        }
        else if (token.TryGetString("RewardAnchor", out anchorStr))
        {
            this.jumpAnchor = Enum.Parse<DestAnchorType>(anchorStr);
        }

        if (string.IsNullOrEmpty(this.unitStrId) == false)
        {
            this.unit = TempletContainer<Unit>.Find(this.unitStrId);
            if (this.unit is null)
            {
                Log.Error($"유닛 템플릿을 찾을 수 없습니다. UnitStrId:{this.unitStrId}");
            }
        }

        if (this.arcpointId > 0)
        {
            this.arcpoint = TempletContainer<LobbyItem>.Find(this.arcpointId);
            if (this.arcpoint is null)
            {
                Log.Error($"로비 아이템 템플릿을 찾을 수 없습니다. ArcpointId:{this.arcpointId}");
            }
        }

        // 컬렉션의 요소들이 변경될 때 UnitNames로 바인딩한 값들도 새로고침 하도록 알림 추가.
        this.unitNames.CollectionChanged += (s, e) => this.OnPropertyChanged(nameof(this.UnitNames));
    }

    public long Uid { get; private set; }
    public L10nText UnitTalk => this.unitTalk;
    public IList<ChoiceOption> Choices => this.choices;
    public IList<string> UnitNames => this.unitNames;

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

    public LobbyItem? Arcpoint
    {
        get => this.arcpoint;
        set => this.SetProperty(ref this.arcpoint, value);
    }

    public CutsceneUnitPos UnitPos
    {
        get => this.unitPos;
        set => this.SetProperty(ref this.unitPos, value);
    }

    public bool TalkAppend
    {
        get => this.talkAppend;
        set => this.SetProperty(ref this.talkAppend, value);
    }

    public DestAnchorType JumpAnchor
    {
        get => this.jumpAnchor;
        set => this.SetProperty(ref this.jumpAnchor, value);
    }

    public string? UnitMotion
    {
        get => this.unitMotion;
        set => this.SetProperty(ref this.unitMotion, value);
    }

    public TransitionEffect? TransitionEffect
    {
        get => this.transitionEffect;
        set => this.SetProperty(ref this.transitionEffect, value);
    }

    public TransitionControl? TransitionControl
    {
        get => this.transitionControl;
        set => this.SetProperty(ref this.transitionControl, value);
    }

    public string? StartBgmFileName
    {
        get => this.startBgmFileName;
        set => this.SetProperty(ref this.startBgmFileName, value);
    }

    public string? StartFxSoundName
    {
        get => this.startFxSoundName;
        set => this.SetProperty(ref this.startFxSoundName, value);
    }

    public string? EndBgmFileName
    {
        get => this.endBgmFileName;
        set => this.SetProperty(ref this.endBgmFileName, value);
    }

    public string? EndFxSoundName
    {
        get => this.endFxSoundName;
        set => this.SetProperty(ref this.endFxSoundName, value);
    }

    public EmotionEffect EmotionEffect
    {
        get => this.emotionEffect;
        set => this.SetProperty(ref this.emotionEffect, value);
    }

    public string? TalkVoice
    {
        get => this.talkVoice;
        set => this.SetProperty(ref this.talkVoice, value);
    }

    public bool WaitClick
    {
        get => this.waitClick;
        set => this.SetProperty(ref this.waitClick, value);
    }

    public float WaitTime
    {
        get => this.waitTime;
        set => this.SetProperty(ref this.waitTime, value);
    }

    public string? BgFileName
    {
        get => this.bgFileName;
        set => this.SetProperty(ref this.bgFileName, value);
    }

    public float BgChangeTime
    {
        get => this.bgChangeTime;
        set => this.SetProperty(ref this.bgChangeTime, value);
    }

    public bool UnitQuickSet
    {
        get => this.unitQuickSet;
        set => this.SetProperty(ref this.unitQuickSet, value);
    }

    public CutsceneAutoHighlight AutoHighlight
    {
        get => this.autoHighlight;
        set => this.SetProperty(ref this.autoHighlight, value);
    }

    public CutsceneFilterType FilterType
    {
        get => this.filterType;
        set => this.SetProperty(ref this.filterType, value);
    }

    public CutsceneClearType CutsceneClear
    {
        get => this.cutsceneClear;
        set => this.SetProperty(ref this.cutsceneClear, value);
    }

    public float BgFlashBang
    {
        get => this.bgFlashBang;
        set => this.SetProperty(ref this.bgFlashBang, value);
    }

    public float BgCrash
    {
        get => this.bgCrash;
        set => this.SetProperty(ref this.bgCrash, value);
    }

    public float BgCrashTime
    {
        get => this.bgCrashTime;
        set => this.SetProperty(ref this.bgCrashTime, value);
    }

    public bool HasBgFlashCrashData =>
        this.bgFlashBang > 0f ||
        this.bgCrash > 0f ||
        this.bgCrashTime > 0f;

    public CutsceneSoundLoopControl StartFxLoopControl
    {
        get => this.startFxLoopControl;
        set => this.SetProperty(ref this.startFxLoopControl, value);
    }

    public CutsceneSoundLoopControl EndFxLoopControl
    {
        get => this.endFxLoopControl;
        set => this.SetProperty(ref this.endFxLoopControl, value);
    }

    public SlateControlType SlateControlType
    {
        get => this.slateControlType;
        set => this.SetProperty(ref this.slateControlType, value);
    }

    public int SlateSectionNo
    {
        get => this.slateSectionNo;
        set => this.SetProperty(ref this.slateSectionNo, value);
    }

    public bool HasSlateControlData =>
        this.slateControlType != SlateControlType.NONE ||
        this.slateSectionNo > 0;

    public BgFadeInOut? BgFadeInOut
    {
        get => this.bgFadeInOut;
        set => this.SetProperty(ref this.bgFadeInOut, value);
    }

    public TalkPositionControlType TalkPositionControl
    {
        get => this.talkPositionControl;
        set => this.SetProperty(ref this.talkPositionControl, value);
    }

    public string? AmbientSound
    {
        get => this.ambientSound;
        set => this.SetProperty(ref this.ambientSound, value);
    }

    public object ToOutputType()
    {
        var result = new CutOutputFormat
        {
            Uid = this.Uid,
            ContentsTag = this.contentsTag,
            CutsceneStrId = this.cutsceneStrId,
            WaitClick = this.waitClick,
            WaitTime = this.waitTime,
            BgFlashBang = CutOutputFormat.EliminateZero(this.bgFlashBang),
            BgCrash = CutOutputFormat.EliminateZero(this.bgCrash),
            BgCrashTime = CutOutputFormat.EliminateZero(this.bgCrashTime),
            EndBgmFileName = this.endBgmFileName,
            BgFileName = this.bgFileName,
            StartBgmFileName = this.startBgmFileName,
            StartFxSoundName = this.startFxSoundName,
            CutsceneClear = EliminateEnum(this.cutsceneClear, CutsceneClearType.NONE),
            UnitStrId = this.unit?.StrId,
            UnitQuickSet = EliminateFalse(this.unitQuickSet),
            UnitPos = EliminateEnum(this.unitPos, CutsceneUnitPos.NONE),
            CameraOffset = this.cameraOffset,
            CameraOffsetTime = this.cameraOffsetTime,
            EmotionEffect = EliminateEnum(this.emotionEffect, EmotionEffect.NONE),
            UnitTalk_KOR = this.unitTalk.AsNullable(L10nType.Korean),
            UnitTalk_ENG = this.unitTalk.AsNullable(L10nType.English),
            UnitTalk_JPN = this.unitTalk.AsNullable(L10nType.Japanese),
            UnitTalk_CHN = this.unitTalk.AsNullable(L10nType.ChineseSimplified),
            TalkTime = CutOutputFormat.EliminateZero(this.talkTime),
            TalkPositionControl = EliminateEnum(this.talkPositionControl, TalkPositionControlType.NONE),
            TalkAppend = EliminateFalse(this.talkAppend),
            UnitMotion = this.unitMotion,
            TransitionEffect = this.transitionEffect,
            TransitionControl = this.transitionControl,
            TalkVoice = this.talkVoice,
            BgChangeTime = CutOutputFormat.EliminateZero(this.bgChangeTime),
            AutoHighlight = EliminateEnum(this.autoHighlight, CutsceneAutoHighlight.NONE),
            FilterType = EliminateEnum(this.filterType, CutsceneFilterType.NONE),
            ArcpointId = this.arcpoint?.Id,
            StartFxSoundLoopControl = EliminateEnum(this.startFxLoopControl, CutsceneSoundLoopControl.NONE),
            EndFxLoopControl = EliminateEnum(this.endFxLoopControl, CutsceneSoundLoopControl.NONE),
            SlateControlType = EliminateEnum(this.slateControlType, SlateControlType.NONE),
            SlateSectionNo = EliminateZeroInt(this.slateSectionNo),
            AmbientSound = this.ambientSound,
        };

        this.bgFadeInOut?.WriteTo(result);

        if (this.jumpAnchor != DestAnchorType.None)
        {
            if (this.jumpAnchor == DestAnchorType.REWARD_ANCHOR_1)
            {
                result.RewardAnchor = this.jumpAnchor;
            }
            else
            {
                result.JumpAnchorInfo = this.jumpAnchor;
            }
        }

        if (this.choices.Count > 0)
        {
            result.JumpAnchorData = this.choices.Select(e => e.ToOutputType()).ToArray();
        }

        if (this.unitNames.Count > 0)
        {
            result.UnitNameString = this.unitNames.ToArray();
        }

        return result;

        static int? EliminateZeroInt(int source)
        {
            return source > 0 ? source : null;
        }

        static bool? EliminateFalse(bool source)
        {
            return source ? source : null;
        }

        static T? EliminateEnum<T>(T source, T defaultValue) where T : struct, Enum
        {
            return source.Equals(defaultValue) ? null : source;
        }
    }

    public string GetSummaryText()
    {
        if (this.choices.Count > 0)
        {
            var list = string.Join(Environment.NewLine, this.choices.Select(e => e.GetSummaryText()));
            return $"[선택지] {Environment.NewLine}{list}";
        }

        return this.unitTalk.Korean;
    }

    public void SetUid(long uid)
    {
        if (this.Uid != 0)
        {
            Log.Warn($"기존 uid가 새로운 값으로 변경됩니다. uid:{this.Uid} -> {uid}");
        }

        this.Uid = uid;
    }

    public bool HasScreenBoxData()
    {
        return this.transitionControl is not null ||
            this.TransitionEffect is not null ||
            this.bgFileName is not null ||
            this.filterType != CutsceneFilterType.NONE ||
            this.HasBgFlashCrashData ||
            this.HasSlateControlData ||
            this.bgFadeInOut is not null ||
            this.bgChangeTime > 0f;
    }

    public bool HasUnitData()
    {
        return this.unitTalk.HasData ||
            this.unitNames.Count > 0 ||
            string.IsNullOrEmpty(this.startBgmFileName) == false ||
            string.IsNullOrEmpty(this.startFxSoundName) == false ||
            this.startFxLoopControl != CutsceneSoundLoopControl.NONE ||
            string.IsNullOrEmpty(this.endBgmFileName) == false ||
            string.IsNullOrEmpty(this.endFxSoundName) == false ||
            this.endFxLoopControl != CutsceneSoundLoopControl.NONE ||
            this.emotionEffect != EmotionEffect.NONE ||
            this.unit is not null ||
            this.unitPos != CutsceneUnitPos.NONE ||
            string.IsNullOrEmpty(this.unitMotion) == false ||
            string.IsNullOrEmpty(this.talkVoice) == false ||
            string.IsNullOrEmpty(this.ambientSound) == false ||
            this.autoHighlight != CutsceneAutoHighlight.NONE;
    }

    //// --------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.BgFlashBang):
            case nameof(this.BgCrash):
            case nameof(this.BgCrashTime):
                this.OnPropertyChanged(nameof(this.HasBgFlashCrashData));
                break;

            case nameof(this.SlateControlType):
            case nameof(this.SlateSectionNo):
                this.OnPropertyChanged(nameof(this.HasSlateControlData));
                break;
        }
    }
}
