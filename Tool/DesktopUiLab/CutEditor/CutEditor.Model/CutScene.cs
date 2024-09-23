namespace CutEditor.Model;

using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Util;
using static CutEditor.Model.Enums;

public sealed class CutScene : ObservableObject
{
    private readonly L10nText title = new();
    private readonly L10nText shortenTalk = new();
    private int cutsceneId;
    private string fileName = string.Empty;
    private CutsceneType cutsceneType;
    private string cutsceneFilter = string.Empty;
    private string slideTitleIcon = string.Empty;
    private bool titleFadeout;
    private float titleFadeOutTime;
    private float titleTalkTime;
    private float subTitleTalkTime;
    private string shortenBgFileName = string.Empty;

    public CutScene(JsonElement element)
    {
        this.cutsceneId = element.GetInt32("CutsceneId");
        this.fileName = element.GetString("CutsceneFile");
        this.cutsceneType = element.GetEnum<CutsceneType>("CutsceneType");
        this.cutsceneFilter = element.GetString("CutsceneFilter");
        this.slideTitleIcon = element.GetString("SlideTitleIcon");
        this.titleFadeout = element.GetBoolean("TitleFadeout");
        this.titleFadeOutTime = element.GetFloat("TitleFadeOutTime");
        this.title.Load(element, "Title");
        this.titleTalkTime = element.GetFloat("TitleTalkTime");
        this.subTitleTalkTime = element.GetFloat("SubTitleTalkTime");
        this.shortenBgFileName = element.GetString("ShortenBgFileName");
        this.shortenTalk.Load(element, "ShortenTalk");
    }
}
