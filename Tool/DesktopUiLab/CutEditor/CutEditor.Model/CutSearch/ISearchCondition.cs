namespace CutEditor.Model.CutSearch;

public interface ISearchCondition
{
    bool IsValid { get; }
    bool IsTarget(Cut cut);
}
