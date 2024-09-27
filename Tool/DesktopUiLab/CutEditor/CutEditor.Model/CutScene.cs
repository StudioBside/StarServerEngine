namespace CutEditor.Model;

using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Du.Core.Util;
using Newtonsoft.Json.Linq;
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
}
