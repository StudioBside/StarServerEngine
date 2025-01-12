namespace CutEditor.Model;

using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using static StringStorage.Enums;

public sealed class L10nText : ObservableObject, ISearchable, IL10nText
{
    private readonly string[] values = new string[EnumUtil<L10nType>.Count];

    public L10nText()
    {
    }

    public L10nText(string defaultText)
    {
        this.values[(int)L10nType.Kor] = defaultText;
    }

    public string Korean
    {
        get => this.values[(int)L10nType.Kor];
        set => this.SetAndNotify(L10nType.Kor, value);
    }

    public string English
    {
        get => this.values[(int)L10nType.Eng];
        set => this.SetAndNotify(L10nType.Eng, value);
    }

    public string Japanese
    {
        get => this.values[(int)L10nType.Jpn];
        set => this.SetAndNotify(L10nType.Jpn, value);
    }

    public string ChineseSimplified
    {
        get => this.values[(int)L10nType.ChnS];
        set => this.SetAndNotify(L10nType.ChnS, value);
    }

    public string ChineseTraditional
    {
        get => this.values[(int)L10nType.ChnT];
        set => this.SetAndNotify(L10nType.ChnT, value);
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

    public string Set(L10nType l10nType, string value)
    {
        var oldValue = this.values[(int)l10nType];
        this.values[(int)l10nType] = value;

        this.OnPropertyChanged(l10nType.ToString());
        return oldValue;
    }

    public override string ToString() => this.values[(int)L10nType.Kor];

    internal void Load(JToken token, string prefix)
    {
        for (int i = 0; i < this.values.Length; ++i)
        {
            var l10nType = (L10nType)i;
            var key = $"{prefix}_{l10nType.ToString().ToUpper()}";
            this.values[i] = token.GetString(key, string.Empty);
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
