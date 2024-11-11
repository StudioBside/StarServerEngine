namespace Du.Core.Interfaces;

/// <summary>
/// 사용자에게 UI를 통해 에러 메시지를 노출하는 역할.
/// </summary>
public interface IPopupMessageNotifier
{
    void NotifyError(string message);
    void Notify(string title, string message);
}
