namespace Du.Presentation.Util;

using System.Windows;
using Cs.Core.Core;
using Du.Core.Interfaces;
using Du.Core.Util;

public sealed class ClipboardWrapper : IClipboardWrapper
{
    private AtomicFlag setAccessFlag = new(initialValue: false);

    public bool ContainsText() => Clipboard.ContainsText();
    public string GetText() => Clipboard.GetText();
    public void SetText(string text)
    {
        using var guard = ReEnterGuard.TryEnter(this.setAccessFlag);
        if (guard is null)
        {
            return;
        }

        // Clipboard에 lock이 해제되기 전에 호출되면 0x800401D0(CLIPBRD_E_CANT_OPEN) 예외가 발생합니다.
        Clipboard.SetText(text);
    }
}