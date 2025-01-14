namespace CutEditor.Model.L10n;

using Shared.Interfaces;
using static CutEditor.Model.Enums;

/// <summary>
/// 번역 데이터 타입에 따라 다른 구현을 추상화하는 인터페이스. 
/// </summary>
public interface IL10nMapping
{
    string UidStr { get; }
    IL10nText SourceData { get; }
    IL10nText? ImportedData { get; }
    L10nMappingState MappingState { get; }
}