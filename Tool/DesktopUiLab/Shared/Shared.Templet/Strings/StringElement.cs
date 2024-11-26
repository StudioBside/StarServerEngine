namespace Shared.Templet.Strings
{
    using System;
    using System.Collections.Generic;
    using Cs.Core.Util;
    using Cs.Logging;
    using Newtonsoft.Json.Linq;
    using Shared.Interfaces;
    using static Shared.Templet.Enums;

    public sealed class StringElement : ISearchable
    {
        private readonly List<string> keys = new();
        private readonly string[] values = new string[EnumUtil<L10nType>.Count];

        public StringElement(JToken token)
        {
            if (token.TryGetString("Key", out var key))
            {
                this.keys.Add(key);
            }
            else if (token.TryGetArray("Keys", this.keys) == false)
            {
                ErrorContainer.Add($"StringElement: Key or Keys is not found in {token}");
                return;
            }

            for (int i = 0; i < this.values.Length; ++i)
            {
                var l10nType = (L10nType)i;
                // note: 한국어는 'Value', 나머지 언어는 'Value_ENG' 형식으로 저장되어 있음
                var dataKey = l10nType == L10nType.Korean ? "Value" : l10nType.ToJsonKey("Value");
                this.values[i] = token.GetString(dataKey, string.Empty);
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
        }

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
    }
}
