namespace CutEditor.Model;

using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using static Shared.Templet.Enums;

public sealed class L10nText : ObservableObject, ISearchable
{
    private readonly string[] values = new string[EnumUtil<L10nType>.Count];

    public string Korean
    {
        get => this.values[(int)L10nType.Korean];
        set => this.SetAndNotify(L10nType.Korean, value);
    }

    public string English
    {
        get => this.values[(int)L10nType.English];
        set => this.SetAndNotify(L10nType.English, value);
    }

    public string Japanese
    {
        get => this.values[(int)L10nType.Japanese];
        set => this.SetAndNotify(L10nType.Japanese, value);
    }

    public string ChineseSimplified
    {
        get => this.values[(int)L10nType.ChineseSimplified];
        set => this.SetAndNotify(L10nType.ChineseSimplified, value);
    }

    public string ChineseTraditional
    {
        get => this.values[(int)L10nType.ChineseTraditional];
        set => this.SetAndNotify(L10nType.ChineseTraditional, value);
    }

    public bool HasData => this.values.Any(e => string.IsNullOrEmpty(e) == false);

    public bool IsTarget(string keyword)
    {
        foreach (var value in this.values.WhereNotNull())
        {
            if (value.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public string Get(L10nType l10nType)
    {
        return this.values[(int)l10nType];
    }

    public override string ToString() => this.values[(int)L10nType.Korean];

    internal void Load(JToken token, string prefix)
    {
        for (int i = 0; i < this.values.Length; ++i)
        {
            var l10nType = (L10nType)i;
            this.values[i] = token.GetString(l10nType.ToJsonKey(prefix), string.Empty);
        }
    }

    internal string? AsNullable(L10nType l10nType)
    {
        return string.IsNullOrEmpty(this.values[(int)l10nType])
            ? null : this.values[(int)l10nType];
    }

    //// --------------------------------------------------------------------------------------------

    private void SetAndNotify(L10nType l10nType, string value, [CallerMemberName] string? propertyName = null)
    {
        this.values[(int)l10nType] = value;
        this.OnPropertyChanged(propertyName);
    }
}
