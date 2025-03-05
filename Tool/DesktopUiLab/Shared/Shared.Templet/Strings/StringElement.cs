namespace Shared.Templet.Strings
{
    using System;
    using System.Collections.Generic;
    using Cs.Core.Util;
    using Cs.Logging;
    using Newtonsoft.Json.Linq;
    using Shared.Interfaces;
    using StringStorage.Translation;
    using static StringStorage.Enums;

    public sealed class StringElement : ISearchable
    {
        private readonly List<string> keys = new();
        private readonly string[] values = new string[EnumUtil<L10nType>.Count];

        public StringElement(
            JToken token,
            string categoryName,
            string groupName,
            L10nReadOnlyDb? l10nDb)
        {
            this.CategoryName = categoryName;
            this.GroupName = groupName;

            if (token.TryGetString("Tag", out var tag))
            {
                this.Tag = tag;
            }

            if (token.TryGetString("Key", out var key))
            {
                this.keys.Add(key);
            }
            else if (token.TryGetArray("Keys", this.keys) == false)
            {
                ErrorContainer.Add($"StringElement: Key or Keys is not found in {token}");
                return;
            }

            // 한국어 데이터만 json에서 읽는다.
            this.values[(int)L10nType.Korean] = token.GetString("Value");

            // 나머지는 DB에서 읽는다.
            var textSet = l10nDb?.Get(this.values[(int)L10nType.Korean]);
            for (int i = 1; i < this.values.Length; ++i)
            {
                var l10nType = (L10nType)i;
                this.values[i] = textSet?.GetValueString(l10nType) ?? string.Empty;
            }
        }

        public StringElement(string primeKey, string koreanText)
        {
            this.keys.Add(primeKey);
            this.values[(int)L10nType.Korean] = koreanText;
            for (int i = 0; i < this.values.Length; ++i)
            {
                if (this.values[i] is null)
                {
                    this.values[i] = string.Empty;
                }
            }

            this.CategoryName = string.Empty;
            this.GroupName = string.Empty;
        }

        public string CategoryName { get; }
        public string GroupName { get; }
        public string Tag { get; } = string.Empty;
        public string PrimeKey => this.keys.First();
        public int KeyCount => this.keys.Count;
        public IEnumerable<string> Keys => this.keys;
        public string Korean => this.values[(int)L10nType.Korean];
        public string English => this.values[(int)L10nType.English];
        public string Japanese => this.values[(int)L10nType.Japanese];
        public string ChineseSimplified => this.values[(int)L10nType.ChineseSimplified];
        public string ChineseTraditional => this.values[(int)L10nType.ChineseTraditional];
        public bool IsAlphaNumeric => this.Korean.All(c => char.IsAscii(c) || char.IsWhiteSpace(c));

        public override string ToString() => this.Korean;

        bool ISearchable.IsTarget(string keyword)
        {
            return this.keys.Any(e => e.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                this.values.Any(e => e.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        public string Get(L10nType l10nType)
        {
            return this.values[(int)l10nType];
        }

        public string Set(L10nType l10nType, string value)
        {
            var oldValue = this.values[(int)l10nType];
            this.values[(int)l10nType] = value;
            return oldValue;
        }
    }
}
