namespace CutEditor.Model;

using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Du.Core.Util;
using static CutEditor.Model.Enums;

internal sealed class L10nText : ObservableObject
{
    private readonly string[] values = new string[EnumUtil<L10nType>.Count];

    internal void Load(JsonElement element, string prefix)
    {
        for (int i = 0; i < this.values.Length; ++i)
        {
            var l10nType = (L10nType)i;
            this.values[i] = element.GetString(l10nType.ToJsonKey(prefix), string.Empty);
        }
    }
}
