namespace Du.Presentation.Util;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Du.Core.Interfaces;

/// <summary>
/// frame의 page는 계속 변경되는데, 종료 이벤트 수신을 변경시마다 매번 등록하는 것은 번거롭기 때문에
/// 이벤트 핸들러는 여기 한 곳에서만 고정 등록하고 vm에는 frame의 현재 content를 확인하여 이벤트를 전달합니다.
/// </summary>
public class AppExitNotifier
{
    private Frame frame = null!;

    public void SetFrame(Frame frame)
    {
        this.frame = frame;

        //Application.Current.Exit += this.OnExit;
        Application.Current.MainWindow.Closing += this.OnClosing;
    }

    //// ------------------------------------------------------------------

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (this.frame.Content is Page pageContent &&
            pageContent.DataContext is IAppExitHandler handler)
        {
            handler.OnClosing(e);
        }
    }
}
