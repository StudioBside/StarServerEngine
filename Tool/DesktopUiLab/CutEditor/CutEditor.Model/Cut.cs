namespace CutEditor.Model;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model.Detail;
using CutEditor.Model.ExcelFormats;
using CutEditor.Model.Preview;
using Newtonsoft.Json.Linq;
using NKM;
using Shared.Templet.Base;
using Shared.Templet.Strings;
using Shared.Templet.TempletTypes;
using static CutEditor.Model.Enums;
using static CutEditor.Model.Messages;
using static Du.Core.Messages;
using static NKM.NKMOpenEnums;
using static StringStorage.Enums;

public sealed class Cut : ObservableObject
{
    public const float TalkTimeDefault = 0.03f;

    private readonly L10nText unitTalk = new();
    private readonly ObservableCollection<ChoiceOption> choices = new();
    private readonly ObservableCollection<StringElement> unitNames = new();
    private readonly CutPreview preview;
    private readonly bool constructorFinished;
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
    private UnitIdConst? unitIdConst;
    private bool unitQuickSet;
    private CutsceneUnitPos unitPos;
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
    private string? cutEffect;
    private UnitEffect unitEffect;
    private bool nicknameInput;
    private CameraOffset cameraOffset;
    private Ease cameraEase;
    private CameraOffsetTime cameraOffsetTime;
    #region ImageName
    private ImageNameFadeEffect imageNameFadeIn;
    private ImageNameFadeEffect imageNameFadeOut;
    private string? imageName;
    #endregion
    #region BgControl
    private bool bgAniScale;
    private string[]? bgOffsetScale; // NKMVector3
    private float bgOffsetScaleTime;
    private bool bgAniPos;
    private string[]? bgPos; // NKMVector2
    private Ease bgEase;
    private float bgPosTime;
    #endregion

    public Cut(long uid)
    {
        this.Uid = uid;
        this.preview = new CutPreview(this);

        // 컬렉션의 요소들이 변경될 때 UnitNames로 바인딩한 값들도 새로고침 하도록 알림 추가.
        this.unitNames.CollectionChanged += (s, e) => this.OnPropertyChanged(nameof(this.UnitNames));

        // unktTalk 변경될 때 talkTime 값을 변경하기 위해 이벤트 수신
        this.unitTalk.PropertyChanged += this.UnitTalk_PropertyChanged;

        this.constructorFinished = true;
    }

    public Cut(JToken token, string debugName) : this(token.GetInt64("Uid", 0))
    {
        this.constructorFinished = false;

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
        this.cameraOffset = token.GetEnum("CameraOffset", CameraOffset.NONE);
        this.cameraEase = token.GetEnum("CameraEase", Ease.Unset);
        this.cameraOffsetTime = token.GetEnum("CameraOffsetTime", CameraOffsetTime.NONE);

        this.emotionEffect = token.GetEnum("EmotionEffect", EmotionEffect.NONE);
        this.unitTalk.Load(token, "UnitTalk");
        this.talkTime = token.GetFloat("TalkTime", 0f);
        this.unitStrId = token.GetString("UnitStrId", null!);
        this.talkPositionControl = token.GetEnum("TalkPositionControl", TalkPositionControlType.NONE);
        token.TryGetArray("JumpAnchorData", this.choices, ChoiceOption.Load);
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
        this.cutEffect = token.GetString("CutEffect", null!);
        this.unitEffect = token.GetEnum("UnitEffect", UnitEffect.NONE);
        this.nicknameInput = token.GetBool("NicknameInput", false);
        this.imageNameFadeIn = token.GetEnum("ImageNameFadeIn", ImageNameFadeEffect.NONE);
        this.imageNameFadeOut = token.GetEnum("ImageNameFadeOut", ImageNameFadeEffect.NONE);
        this.imageName = token.GetString("ImageName", null!);
        this.bgAniScale = token.GetBool("BgAniScale", false);
        this.bgOffsetScaleTime = token.GetFloat("BgOffsetScaleTime", 0f);
        this.bgAniPos = token.GetBool("BgAniPos", false);
        this.bgEase = token.GetEnum("BgEase", Ease.Unset);
        this.bgPosTime = token.GetFloat("BgPosTime", 0f);
        var buffer = new List<string>();

        if (token.TryGetArray("UnitNameString", buffer))
        {
            foreach (var data in buffer)
            {
                if (StringTable.Instance.TryGetElement(data, out var element) == false)
                {
                    Log.Error($"유효하지 않은 unitName 입니다:{data}");
                    continue;
                }

                this.unitNames.Add(element);
            }
        }

        if (token.TryGetArray("BgOffsetScale", buffer))
        {
            this.bgOffsetScale = buffer.ToArray();
        }

        if (token.TryGetArray("BgPos", buffer))
        {
            this.bgPos = buffer.ToArray();
        }

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
            if (Enum.TryParse<UnitIdConst>(this.unitStrId, out var unitIdConst))
            {
                this.unitIdConst = unitIdConst;
            }
            else
            {
                this.unit = TempletContainer<Unit>.Find(this.unitStrId);
                if (this.unit is null)
                {
                    Log.Error($"{debugName} 유닛 템플릿을 찾을 수 없습니다. UnitStrId:{this.unitStrId}");
                }
            }
        }

        if (this.arcpointId > 0)
        {
            this.arcpoint = TempletContainer<LobbyItem>.Find(this.arcpointId);
            if (this.arcpoint is null)
            {
                Log.Error($"{debugName} 로비 아이템 템플릿을 찾을 수 없습니다. ArcpointId:{this.arcpointId}");
            }
        }

        this.constructorFinished = true;
    }

    public long Uid { get; private set; }
    public L10nText UnitTalk => this.unitTalk;
    public IList<ChoiceOption> Choices => this.choices;
    public IList<StringElement> UnitNames => this.unitNames;
    public CutPreview Preview => this.preview;

    public float TalkTime
    {
        get => this.talkTime;
        set => this.SetProperty(ref this.talkTime, value);
    }

    public Unit? Unit
    {
        get => this.unit;
        set => this.SetProperty(this.unit, value, this.OnSetUnit);
    }

    public UnitIdConst? UnitIdConst
    {
        get => this.unitIdConst;
        set => this.SetProperty(this.unitIdConst, value, this.OnSetUnitIdConst);
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

    public bool HasMinorityData =>
        string.IsNullOrEmpty(this.cutEffect) == false ||
        this.unitEffect != UnitEffect.NONE ||
        this.nicknameInput;

    public string? CutEffect
    {
        get => this.cutEffect;
        set => this.SetProperty(ref this.cutEffect, value);
    }

    public UnitEffect UnitEffect
    {
        get => this.unitEffect;
        set => this.SetProperty(ref this.unitEffect, value);
    }

    public bool NicknameInput
    {
        get => this.nicknameInput;
        set => this.SetProperty(ref this.nicknameInput, value);
    }

    public CameraOffset CameraOffset
    {
        get => this.cameraOffset;
        set => this.SetProperty(ref this.cameraOffset, value);
    }

    public Ease CameraEase
    {
        get => this.cameraEase;
        set => this.SetProperty(ref this.cameraEase, value);
    }

    public CameraOffsetTime CameraOffsetTime
    {
        get => this.cameraOffsetTime;
        set => this.SetProperty(ref this.cameraOffsetTime, value);
    }

    public string? SpeakerNameKor => this.GetSpeakerName(L10nType.Korean);
    public string? SpeakerNameJpn => this.GetSpeakerName(L10nType.Japanese);

    private bool HasUnitId => this.unit is not null || this.unitIdConst is not null;
    private string? FinalUnitId => this.unit?.StrId ?? this.unitIdConst?.ToString();

    public object ToOutputJsonType()
    {
        var result = new CutOutputJsonFormat
        {
            Uid = this.Uid,
            ContentsTag = this.contentsTag,
            CutsceneStrId = this.cutsceneStrId,
            WaitClick = this.waitClick,
            WaitTime = this.waitTime,
            BgFlashBang = CutOutputJsonFormat.EliminateZero(this.bgFlashBang),
            BgCrash = CutOutputJsonFormat.EliminateZero(this.bgCrash),
            BgCrashTime = CutOutputJsonFormat.EliminateZero(this.bgCrashTime),
            EndBgmFileName = this.endBgmFileName,
            BgFileName = this.bgFileName,
            StartBgmFileName = this.startBgmFileName,
            StartFxSoundName = this.startFxSoundName,
            CutsceneClear = OutputTransfer.EliminateEnum(this.cutsceneClear, CutsceneClearType.NONE),
            UnitStrId = this.FinalUnitId,
            UnitQuickSet = OutputTransfer.EliminateFalse(this.unitQuickSet),
            UnitPos = OutputTransfer.EliminateEnum(this.unitPos, CutsceneUnitPos.NONE),
            CameraOffset = OutputTransfer.EliminateEnum(this.cameraOffset, CameraOffset.NONE),
            CameraEase = OutputTransfer.EliminateEnum(this.cameraEase, Ease.Unset),
            CameraOffsetTime = OutputTransfer.EliminateEnum(this.cameraOffsetTime, CameraOffsetTime.NONE),
            EmotionEffect = OutputTransfer.EliminateEnum(this.emotionEffect, EmotionEffect.NONE),
            UnitTalk_KOR = this.unitTalk.AsNullable(L10nType.Korean),
            UnitTalk_ENG = this.unitTalk.AsNullable(L10nType.English),
            UnitTalk_JPN = this.unitTalk.AsNullable(L10nType.Japanese),
            UnitTalk_CHN = this.unitTalk.AsNullable(L10nType.ChineseSimplified),
            TalkTime = CutOutputJsonFormat.EliminateZero(this.talkTime),
            TalkPositionControl = OutputTransfer.EliminateEnum(this.talkPositionControl, TalkPositionControlType.NONE),
            TalkAppend = OutputTransfer.EliminateFalse(this.talkAppend),
            UnitMotion = this.unitMotion,
            TransitionEffect = this.transitionEffect,
            TransitionControl = this.transitionControl,
            TalkVoice = this.talkVoice,
            BgChangeTime = CutOutputJsonFormat.EliminateZero(this.bgChangeTime),
            AutoHighlight = OutputTransfer.EliminateEnum(this.autoHighlight, CutsceneAutoHighlight.NONE),
            FilterType = OutputTransfer.EliminateEnum(this.filterType, CutsceneFilterType.NONE),
            ArcpointId = this.arcpoint?.Id,
            StartFxSoundLoopControl = OutputTransfer.EliminateEnum(this.startFxLoopControl, CutsceneSoundLoopControl.NONE),
            EndFxLoopControl = OutputTransfer.EliminateEnum(this.endFxLoopControl, CutsceneSoundLoopControl.NONE),
            SlateControlType = OutputTransfer.EliminateEnum(this.slateControlType, SlateControlType.NONE),
            SlateSectionNo = OutputTransfer.EliminateZeroInt(this.slateSectionNo),
            AmbientSound = this.ambientSound,
            ImageNameFadeIn = OutputTransfer.EliminateEnum(this.imageNameFadeIn, ImageNameFadeEffect.NONE),
            ImageNameFadeOut = OutputTransfer.EliminateEnum(this.imageNameFadeOut, ImageNameFadeEffect.NONE),
            ImageName = this.imageName,
            BgAniScale = OutputTransfer.EliminateFalse(this.bgAniScale),
            BgOffsetScale = this.bgOffsetScale,
            BgOffsetScaleTime = CutOutputJsonFormat.EliminateZero(this.bgOffsetScaleTime),
            BgAniPos = OutputTransfer.EliminateFalse(this.bgAniPos),
            BgPos = this.bgPos,
            BgEase = OutputTransfer.EliminateEnum(this.bgEase, Ease.Unset),
            BgPosTime = CutOutputJsonFormat.EliminateZero(this.bgPosTime),
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
            result.UnitNameString = this.unitNames.Select(e => e.PrimeKey).ToArray();
        }

        return result;
    }

    public IEnumerable<CutOutputExcelFormat> ToOutputExcelType()
    {
        if (this.choices.Count > 0)
        {
            foreach (var choice in this.choices)
            {
                yield return new CutOutputExcelFormat(choice);
            }

            yield break;
        }

        yield return new CutOutputExcelFormat(this);
    }

    public IEnumerable<ShortenCutOutputExcelFormat> ToShortenOutputExcelType(string fileName)
    {
        if (this.choices.Count > 0)
        {
            foreach (var choice in this.choices)
            {
                yield return new ShortenCutOutputExcelFormat(choice, fileName);
            }

            yield break;
        }

        yield return new ShortenCutOutputExcelFormat(this, fileName);
    }

    public string GetSummaryText(L10nType l10nType)
    {
        if (this.choices.Count > 0)
        {
            return string.Join(Environment.NewLine, this.choices.Select(e => e.GetSummaryText(l10nType)));
        }

        return this.unitTalk.Get(l10nType);
    }

    public void ResetOldDataUid(long uid)
    {
        if (this.Uid != 0)
        {
            Log.Warn($"기존 uid가 새로운 값으로 변경됩니다. uid:{this.Uid} -> {uid}");
        }

        this.Uid = uid;
        foreach (var choice in this.choices)
        {
            choice.InitializeUid(this.Uid, choice.ChoiceUid);
        }
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
            this.HasUnitId ||
            this.unitPos != CutsceneUnitPos.NONE ||
            string.IsNullOrEmpty(this.unitMotion) == false ||
            string.IsNullOrEmpty(this.talkVoice) == false ||
            string.IsNullOrEmpty(this.ambientSound) == false ||
            this.autoHighlight != CutsceneAutoHighlight.NONE;
    }

    public bool HasCameraBoxData()
    {
        return this.cameraOffset != CameraOffset.NONE ||
            this.cameraEase != Ease.Unset ||
            this.cameraOffsetTime != CameraOffsetTime.NONE;
    }

    public override string ToString()
    {
        return $"Cut. Uid:{this.Uid}";
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

            case nameof(this.CutEffect):
            case nameof(this.UnitEffect):
            case nameof(this.NicknameInput):
                this.OnPropertyChanged(nameof(this.HasMinorityData));
                break;

            case nameof(this.CameraOffset):
            case nameof(this.Unit):
            case nameof(this.UnitIdConst):
            case nameof(this.UnitPos):
            case nameof(this.CutsceneClear):
            case nameof(this.BgFileName):
                WeakReferenceMessenger.Default.Send(new UpdatePreviewMessage(this));
                break;

            case nameof(this.TransitionEffect):
                if (this.transitionEffect is not null)
                {
                    this.WaitClick = false;
                }

                break;

            case nameof(this.BgFadeInOut):
                if (this.bgFadeInOut is not null)
                {
                    this.WaitClick = false;
                    this.WaitTime = this.bgFadeInOut.Time;
                }

                break;
        }

        if (this.constructorFinished)
        {
            WeakReferenceMessenger.Default.Send(new DataChangedMessage());
        }
    }

    private void UnitTalk_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(this.UnitTalk.Korean):
                this.TalkTime = string.IsNullOrEmpty(this.UnitTalk.Korean) ? 0f : TalkTimeDefault;
                break;
        }

        WeakReferenceMessenger.Default.Send(new DataChangedMessage());
    }

    private string? GetSpeakerName(L10nType l10nType)
    {
        if (this.unitNames.Count > 0)
        {
            return string.Join(", ", this.unitNames.Select(e => e.Get(l10nType)));
        }

        return this.FinalUnitId;
    }

    private void OnSetUnit(Unit? unit)
    {
        this.unit = unit;
        if (unit is not null)
        {
            this.UnitIdConst = null;
        }
    }

    private void OnSetUnitIdConst(UnitIdConst? unitIdConst)
    {
        this.unitIdConst = unitIdConst;
        if (unitIdConst is not null)
        {
            this.unit = null;
        }
    }
}
