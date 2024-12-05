namespace CutEditor.Model.Interfaces;

public interface ICutsListController
{
    void ScrollIntoView(int index);
    void FocusElement(int index);
}
