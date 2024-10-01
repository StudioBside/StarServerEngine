namespace CutEditor.Model;

using System;
using System.Text;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;

public sealed class Cut
{
    private readonly L10nText unitTalk = new();
    private string unitName = string.Empty;
    private float talkTime;

    public Cut(JToken token)
    {
        this.unitTalk.Load(token, "UnitTalk");
        this.unitName = token.GetString("UnitNameString", string.Empty);
        this.talkTime = token.GetFloat("TalkTime", 0f);
    }
}
