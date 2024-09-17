namespace Du.Core.Interfaces;

/// <summary>
/// 네비게이션 시작, 종료되는 시점을 뷰모델에 알려주는 인터페이스.
/// </summary>
public interface INavigationAware
{
    void OnNavigating(object sender, Uri uri);
    void OnNavigated(object sender, Uri uri);
}
