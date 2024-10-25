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
using static NKM.NKMCutsceneEnums;

public sealed class Cut : ObservableObject
{
    private readonly L10nText unitTalk = new();
    private readonly ObservableCollection<ChoiceOption> choices = new();
    private readonly ObservableCollection<string> unitNames = new();
    private string? contentsTag;
    private string? cutsceneStrId;
    private bool waitClick;
    private float waitTime;
    private Color? bgFadeInStartCol;
    private Color? bgFadeInCol;
    private float bgFadeInTime;
    private Color? bgFadeOutCol;
    private float bgFadeOutTime;
    private float bgFlashBang;
    private float bgCrash;
    private float bgCrashTime;
    private string? endBgmFileName;
    private string? cutsceneClear; // enum
    private string? bgFileName;
    private string? startBgmFileName;
    private string? startFxSoundName;
    private string? emotionEffect; // enum
    private string? unitStrId;
    private Unit? unit;
    private bool unitQuickSet;
    private CutsceneUnitPos unitPos;
    private string? cameraOffset; // enum
    private string? cameraOffsetTime; // enum
    private float talkTime;
    private Color? talkPositionControl; // enum
    private bool talkAppend;
    private JumpAnchorType jumpAnchor;
    private RewardAnchorType rewardAnchor;

    public Cut(JToken token, long uid) : this(uid)
    {
        this.contentsTag = token.GetString("ContentsTag", null!);
        this.cutsceneStrId = token.GetString("CutsceneStrId", null!);
        this.waitClick = token.GetBool("WaitClick", false);
        this.waitTime = token.GetFloat("WaitTime", 0f);
        this.bgFadeInStartCol = LoadColor(token, "BgFadeInStartCol");
        this.bgFadeInCol = LoadColor(token, "BgFadeInCol");
        this.bgFadeInTime = token.GetFloat("BgFadeInTime", 0f);
        this.bgFadeOutCol = LoadColor(token, "BgFadeOutCol");
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
        this.unitPos = token.GetEnum("UnitPos", CutsceneUnitPos.NONE);
        this.cameraOffset = token.GetString("CameraOffset", null!);
        this.cameraOffsetTime = token.GetString("CameraOffsetTime", null!);

        this.emotionEffect = token.GetString("EmotionEffect", null!);
        this.unitTalk.Load(token, "UnitTalk");
        this.talkTime = token.GetFloat("TalkTime", 0f);
        this.unitStrId = token.GetString("UnitStrId", null!);
        this.talkPositionControl = LoadColor(token, "TalkPositionControl");
        token.TryGetArray("JumpAnchorData", this.choices, ChoiceOption.Load);
        token.TryGetArray("UnitNameString", this.unitNames);
        this.talkAppend = token.GetBool("TalkAppend", false);
        this.jumpAnchor = token.GetEnum("JumpAnchorInfo", JumpAnchorType.None);
        this.rewardAnchor = token.GetEnum("RewardAnchor", RewardAnchorType.None);

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

    public JumpAnchorType JumpAnchor
    {
        get => this.jumpAnchor;
        set => this.SetProperty(ref this.jumpAnchor, value);
    }

    public RewardAnchorType RewardAnchor
    {
        get => this.rewardAnchor;
        set => this.SetProperty(ref this.rewardAnchor, value);
    }

    public string? AnchorInfo => this.GetAnchorInfo();

    public object ToOutputType()
    {
        var result = new CutOutputFormat
        {
            Uid = this.Uid,
            ContentsTag = this.contentsTag,
            CutsceneStrId = this.cutsceneStrId,
            WaitClick = this.waitClick,
            WaitTime = this.waitTime,
            BgFadeInStartCol = ConvertColor(this.bgFadeInStartCol),
            BgFadeInCol = ConvertColor(this.bgFadeInCol),
            BgFadeInTime = EliminateZero(this.bgFadeInTime),
            BgFadeOutCol = ConvertColor(this.bgFadeOutCol),
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
            UnitQuickSet = EliminateFalse(this.unitQuickSet),
            UnitPos = EliminateEnum(this.unitPos, CutsceneUnitPos.NONE),
            CameraOffset = this.cameraOffset,
            CameraOffsetTime = this.cameraOffsetTime,
            EmotionEffect = this.emotionEffect,
            UnitTalk_KOR = this.unitTalk.AsNullable(L10nType.Korean),
            UnitTalk_ENG = this.unitTalk.AsNullable(L10nType.English),
            UnitTalk_JPN = this.unitTalk.AsNullable(L10nType.Japanese),
            UnitTalk_CHN = this.unitTalk.AsNullable(L10nType.ChineseSimplified),
            TalkTime = EliminateZero(this.talkTime),
            TalkPositionControl = ConvertColor(this.talkPositionControl),
            TalkAppend = EliminateFalse(this.talkAppend),
            JumpAnchorInfo = EliminateEnum(this.jumpAnchor, JumpAnchorType.None),
            RewardAnchor = EliminateEnum(this.rewardAnchor, RewardAnchorType.None),
        };

        if (this.choices.Count > 0)
        {
            result.JumpAnchorData = this.choices.Select(e => e.ToOutputType()).ToArray();
        }

        if (this.unitNames.Count > 0)
        {
            result.UnitNameString = this.unitNames.ToArray();
        }

        return result;

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

        static T? EliminateEnum<T>(T source, T defaultValue) where T : struct, Enum
        {
            return source.Equals(defaultValue) ? null : source;
        }

        static int[]? ConvertColor(Color? color)
        {
            if (color is null)
            {
                return null;
            }

            return [
                color.Value.R,
                color.Value.G,
                color.Value.B,
                color.Value.A
            ];
        }
    }

    public string GetSummaryText()
    {
        if (this.choices.Count > 0)
        {
            var list = string.Join(Environment.NewLine, this.choices.Select(e => e.Text.Korean));
            return $"[선택지] {Environment.NewLine}{list}";
        }

        return this.unitTalk.Korean;
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

            case nameof(this.JumpAnchor):
            case nameof(this.RewardAnchor):
                this.OnPropertyChanged(nameof(this.AnchorInfo));
                break;
        }
    }

    private static Color? LoadColor(JToken token, string key)
    {
        var buffer = new List<int>();
        if (token.TryGetArray(key, buffer) == false)
        {
            return null;
        }

        if (buffer.Count != 4)
        {
            return null;
        }

        // 데이터에는 RGBA 순서로 들어있고, 아래 생성자는 ARGB 순서로 받습니다.
        return Color.FromArgb(buffer[3], buffer[0], buffer[1], buffer[2]);
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

    private string? GetAnchorInfo()
    {
        if (this.rewardAnchor == RewardAnchorType.None &&
            this.jumpAnchor == JumpAnchorType.None)
        {
            return null;
        }

        var sb = new StringBuilder();
        if (this.rewardAnchor != RewardAnchorType.None)
        {
            sb.Append($"보상: {this.rewardAnchor}");
        }

        if (this.jumpAnchor != JumpAnchorType.None)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }

            sb.Append($"점프: {this.jumpAnchor}");
        }

        return sb.ToString();
    }
}
