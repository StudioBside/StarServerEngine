namespace Du.WpfLib;

using System;
using System.Threading.Tasks;
using System.Windows;

public static class UIThreadHelper
{
    // 현재 스레드가 UI 스레드인지 확인
    public static bool IsUIThread()
    {
        if (Application.Current == null)
        {
            return false;
        }

        return Application.Current.Dispatcher.CheckAccess();
    }

    // UI 스레드에서 작업 실행
    public static void ExecuteOnUIThread(Action action)
    {
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (IsUIThread())
        {
            action();
        }
        else
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }

    // UI 스레드에서 비동기로 작업 실행
    public static async Task ExecuteOnUIThreadAsync(Action action)
    {
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (IsUIThread())
        {
            action();
        }
        else
        {
            await Application.Current.Dispatcher.InvokeAsync(action);
        }
    }

    // 결과값을 반환하는 작업을 UI 스레드에서 실행
    public static T ExecuteOnUIThread<T>(Func<T> func)
    {
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (IsUIThread())
        {
            return func();
        }
        else
        {
            return Application.Current.Dispatcher.Invoke(func);
        }
    }
}