## wpf ui의 네비게이션 기능 확인

* 갤러리 앱의 Navigation 항목 중, BreadCrumbBar 예제와 Multilevel navigation이 유용해 보임

### BreadCrumbBar

* `ObservableCollection<string>` 에 string을 넣고, 거의 모든 기능을 수동으로 제어하는 느낌. 
* 날것으로 제어폭이 넓지만 사용하기에는 불편해 보임.

### Multilevel navigation

* `INavigationService`를 이용한다. 
* `NavigationView`와 함께 쓰인다. 나중에 view까지 같이 쓰는 툴을 만들면 활용하기 좋을 듯.
* navigation을 할 때, 대상 page의 타입(class)를 인자로 사용한다.