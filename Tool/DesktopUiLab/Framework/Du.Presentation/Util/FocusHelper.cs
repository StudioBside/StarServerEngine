namespace Du.Presentation.Util;

using System.Windows;
using System.Windows.Input;

public static class FocusHelper
{
    public static IInputElement? GetFocusedElement()
    {
        return Keyboard.FocusedElement;
    }
}
