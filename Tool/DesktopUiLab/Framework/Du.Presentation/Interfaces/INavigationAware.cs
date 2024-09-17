namespace Du.Presentation.Interfaces;

using System.Windows.Navigation;

/// <summary>
/// 네비게이션 시작, 종료되는 시점을 뷰모델에 알려주는 인터페이스.
/// </summary>
public interface INavigationAware
{
    void OnNavigating(object sender, NavigatingCancelEventArgs navigationEventArgs);
    void OnNavigated(object sender, NavigationEventArgs navigatedEventArgs);
}
