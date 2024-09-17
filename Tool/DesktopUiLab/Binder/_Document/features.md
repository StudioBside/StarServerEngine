## 재사용을 위한 주요 기능

* 앱을 재실행 할 때 이전 창의 상태를 복원 : 프로젝트 설정 사용. MainWindow 클래스 참고
* 

## 외부 패키지

* wpfui : https://wpfui.lepo.co/
* CommunityToolkit.Mvvm : https://learn.microsoft.com/ko-kr/dotnet/communitytoolkit/mvvm/
  * ObservableObject, RelayCommand, IMessenger, IoC, ...
* Microsoft.Extensions.DependencyInjection : https://docs.microsoft.com/ko-kr/dotnet/core/extensions/dependency-injection
  * ServiceCollection
* XamlBehaviorsWpf : https://github.com/Microsoft/XamlBehaviorsWpf
  * Behavior<T>
* MVVMLight : https://www.mvvmlight.net/
  * EventToCommand

## wpf와 무관한 외부 패키지

* Microsoft.Extensions.Configuration : https://docs.microsoft.com/ko-kr/dotnet/core/extensions/configuration
  Microsoft.Extensions.Configuration.Json : https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/
  Microsoft.Extensions.Configuration.Binder : https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder/

.Json이 있어야 appsettings.json을 읽을 수 있다.
.Binder가 있어야 GetValue<T>를 사용할 수 있다.

## 기타 레퍼런스

* https://github.com/kaki104/WpfFramework/
* https://kaki104.tistory.com/806