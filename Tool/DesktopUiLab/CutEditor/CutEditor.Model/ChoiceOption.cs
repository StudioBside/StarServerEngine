namespace CutEditor.Model;

using System;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;

public sealed class ChoiceOption
{
    private readonly L10nText text = new();
   
    public L10nText Text => this.text;

    internal static ChoiceOption? Load(JToken token, int index)
    {
        var result = new ChoiceOption();
        result.text.Load(token, "JumpAnchorStringId");

        return result;
    }

    internal object ToOutputType()
    {
        return new
        {
            JumpAnchorStringId_KOR = this.text.AsNullable(L10nType.Korean),
            JumpAnchorStringId_ENG = this.text.AsNullable(L10nType.English),
            JumpAnchorStringId_JPN = this.text.AsNullable(L10nType.Japanese),
            JumpAnchorStringId_CHN = this.text.AsNullable(L10nType.ChineseSimplified),
        };
    }
}
