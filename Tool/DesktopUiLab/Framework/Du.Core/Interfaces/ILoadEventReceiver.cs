namespace Du.Core.Interfaces;

/// <summary>
/// 페이지 로딩 완료 이벤트를 수신받는 인터페이스.
/// </summary>
public interface ILoadEventReceiver
{
    void OnLoaded();
}
