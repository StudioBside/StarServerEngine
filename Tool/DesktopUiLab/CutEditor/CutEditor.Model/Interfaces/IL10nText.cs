namespace CutEditor.Model.Interfaces;

using static StringStorage.Enums;

public interface IL10nText
{
    string Korean { get; }
    string English { get; }
    string Japanese { get; }
    string ChineseSimplified { get; }
    string ChineseTraditional { get; }

    string Get(L10nType l10nType);
}
