namespace CutEditor.Model;

using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Du.Core.Interfaces;
using Du.Core.Util;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;

public sealed class CutScene : ObservableObject, ISearchable
{
    private readonly int cutsceneId;
    private readonly L10nText title = new();
    private readonly L10nText shortenTalk = new();
    private string fileName = string.Empty;
    private CutsceneType cutsceneType;
    private string cutsceneFilter = string.Empty;
    private string slideTitleIcon = string.Empty;
    private bool titleFadeout;
    private float titleFadeOutTime;
    private float titleTalkTime;
    private float subTitleTalkTime;
    private string shortenBgFileName = string.Empty;

    public CutScene(JToken token)
    {
        this.cutsceneId = token.GetInt32("CutsceneId");
        this.fileName = token.GetString("CutsceneFile");
        this.cutsceneType = token.GetEnum<CutsceneType>("CutsceneType");
        this.cutsceneFilter = token.GetString("CutsceneFilter");
        this.slideTitleIcon = token.GetString("SlideTitleIcon");
        this.titleFadeout = token.GetBool("TitleFadeout");
        this.titleFadeOutTime = token.GetFloat("TitleFadeOutTime");
        this.title.Load(token, "Title");
        this.titleTalkTime = token.GetFloat("TitleTalkTime");
        this.subTitleTalkTime = token.GetFloat("SubTitleTalkTime");
        this.shortenBgFileName = token.GetString("ShortenBgFileName");
        this.shortenTalk.Load(token, "ShortenTalk");
    }

    public int Id => this.cutsceneId;
    public string FileName
    {
        get => this.fileName;
        set => this.SetProperty(ref this.fileName, value);
    }

    public L10nText Title => this.title;
    public L10nText ShortenTalk => this.shortenTalk;

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

    public string SlideTitleIcon
    {
        get => this.slideTitleIcon;
        set => this.SetProperty(ref this.slideTitleIcon, value);
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

    public string ShortenBgFileName
    {
        get => this.shortenBgFileName;
        set => this.SetProperty(ref this.shortenBgFileName, value);
    }

    public bool IsTarget(string keyword)
    {
        return (int.TryParse(keyword, out int id) && this.cutsceneId == id) ||
            this.fileName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
            this.title.IsTarget(keyword) ||
            this.shortenTalk.IsTarget(keyword);
    }

    public override string ToString() => this.title.Korean;
}
