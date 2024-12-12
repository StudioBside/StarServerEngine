namespace Du.Core.Interfaces;

using System.ComponentModel;

/// <summary>
/// 앱이 종료되는 시점을 vm에 알려주는 인터페이스.
/// </summary>
public interface IAppExitHandler
{
    void OnClosing(CancelEventArgs args);
}
