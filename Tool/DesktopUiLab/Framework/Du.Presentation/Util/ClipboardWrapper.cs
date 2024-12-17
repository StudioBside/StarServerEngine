namespace Du.Presentation.Util;

using System.Windows;
using Du.Core.Interfaces;

public sealed class ClipboardWrapper : IClipboardWrapper
{
    public bool ContainsText() => Clipboard.ContainsText();
    public string GetText() => Clipboard.GetText();
    public void SetText(string text) => Clipboard.SetText(text);
}
