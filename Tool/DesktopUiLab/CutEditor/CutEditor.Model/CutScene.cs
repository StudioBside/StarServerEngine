namespace CutEditor.Model;

using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using static CutEditor.Model.Enums;

public sealed class CutScene : ObservableObject, ISearchable
{
    private readonly int cutsceneId;
    private readonly L10nText title = new();
    private string fileName = string.Empty;
    private CutsceneType cutsceneType;
    private string cutsceneFilter = string.Empty;
    private string sideTitleIcon = string.Empty;
    private bool titleFadeout;
    private float titleFadeOutTime;
    private float titleTalkTime;
    private float subTitleTalkTime;

    public CutScene(JToken token)
    {
        this.cutsceneId = token.GetInt32("CutsceneId");
        this.fileName = token.GetString("CutsceneFile");
        this.cutsceneType = token.GetEnum<CutsceneType>("CutsceneType");
        this.cutsceneFilter = token.GetString("CutsceneFilter");
        this.sideTitleIcon = token.GetString("SideTitleIcon");
        this.titleFadeout = token.GetBool("TitleFadeout");
        this.titleFadeOutTime = token.GetFloat("TitleFadeOutTime");
        this.title.Load(token, "Title");
        this.titleTalkTime = token.GetFloat("TitleTalkTime");
        this.subTitleTalkTime = token.GetFloat("SubTitleTalkTime");
    }

    public int Id => this.cutsceneId;
    public string FileName
    {
        get => this.fileName;
        set => this.SetProperty(ref this.fileName, value);
    }

    public L10nText Title => this.title;

    public CutsceneType CutsceneType
    {
        get => this.cutsceneType;
        set => this.SetProperty(ref this.cutsceneType, value);
    }

    public string CutsceneFilter
    {
        get => this.cutsceneFilter;
        set => this.SetProperty(ref this.cutsceneFilter, value);
    }

    public string SideTitleIcon
    {
        get => this.sideTitleIcon;
        set => this.SetProperty(ref this.sideTitleIcon, value);
    }

    public bool TitleFadeout
    {
        get => this.titleFadeout;
        set => this.SetProperty(ref this.titleFadeout, value);
    }

    public float TitleFadeOutTime
    {
        get => this.titleFadeOutTime;
        set => this.SetProperty(ref this.titleFadeOutTime, value);
    }

    public float TitleTalkTime
    {
        get => this.titleTalkTime;
        set => this.SetProperty(ref this.titleTalkTime, value);
    }

    public float SubTitleTalkTime
    {
        get => this.subTitleTalkTime;
        set => this.SetProperty(ref this.subTitleTalkTime, value);
    }

    bool ISearchable.IsTarget(string keyword)
    {
        return (int.TryParse(keyword, out int id) && this.cutsceneId == id) ||
            this.fileName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
            this.title.IsTarget(keyword);
    }

    public override string ToString() => this.title.Korean;
}
