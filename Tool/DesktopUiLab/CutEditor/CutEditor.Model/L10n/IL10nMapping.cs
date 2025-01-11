namespace CutEditor.Model.L10n;

using CutEditor.Model;
using static CutEditor.Model.Enums;
using static StringStorage.Enums;

/// <summary>
/// 번역 데이터 타입에 따라 다른 구현을 추상화하는 인터페이스. 
/// </summary>
public interface IL10nMapping
{
    L10nText L10NText { get; }
    L10nMappingState MappingState { get; }
    string UidStr { get; }

    bool ApplyData(L10nType l10nType);
}