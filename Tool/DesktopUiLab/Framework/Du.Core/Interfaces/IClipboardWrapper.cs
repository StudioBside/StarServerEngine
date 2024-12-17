namespace Du.Core.Interfaces;

public interface IClipboardWrapper
{
    bool ContainsText();
    string GetText();
    void SetText(string text);
}
