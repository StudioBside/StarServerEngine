namespace CutEditor.Model.Detail;

using static CutEditor.Model.Enums;

internal sealed class ChoiceOutputFormat
{
    public long Uid { get; set; }
    public string? JumpAnchorStringId_KOR { get; set; }
    public string? JumpAnchorStringId_ENG { get; set; }
    public string? JumpAnchorStringId_JPN { get; set; }
    public string? JumpAnchorStringId_CHNS { get; set; }
    public StartAnchorType? JumpAnchorId { get; set; }
}