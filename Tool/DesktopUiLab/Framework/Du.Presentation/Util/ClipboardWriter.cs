namespace Du.Presentation.Util;

using System.Windows;
using Du.Core.Interfaces;

public sealed class ClipboardWriter : IClipboardWriter
{
    public void SetText(string text)
    {
        Clipboard.SetText(text);
    }
}
