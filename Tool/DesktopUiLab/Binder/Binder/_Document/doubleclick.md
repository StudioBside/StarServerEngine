## ListView에서 아이템을 더블클릭했을 때 처리

## InvokeCommandAction을 이용한 더블클릭 처리

### 1. System.Windows.Interactivity 추가

먼저, `System.Windows.Interactivity`를 사용하기 위해 NuGet 패키지를 설치한다.

```bash
Install-Package System.Windows.Interactivity.WPF
```

note : `System.Windows.Interactivity`는 deprecated 되었으며, `Microsoft.Xaml.Behaviors.Wpf`로 대체되었다. 

### 2. XAML에 네임스페이스 추가

```xml
xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
```

note: `Microsoft.Xaml.Behaviors.Wpf`를 사용할 경우 다음과 같이 네임스페이스를 추가한다.

```xml
xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
```

### 3. ListView에 InvokeCommandAction 추가

```xml
<ListView ItemsSource="{Binding Items}">
	<i:Interaction.Triggers>
		<i:EventTrigger EventName="MouseDoubleClick">
			<i:InvokeCommandAction 
				Command="{Binding DoubleClickCommand}" 
				CommandParameter="{Binding SelectedItem, RelativeSource={RelativeSource AncestorType=ListView}}"/>
		</i:EventTrigger>
	</i:Interaction.Triggers>
</ListView>
```

### 4. ViewModel에 DoubleClickCommand 추가

```csharp
public ICommand DoubleClickCommand { get; }

public MainViewModel()
{
	DoubleClickCommand = new RelayCommand<object>(DoubleClick);
}

private void DoubleClick(object obj)
{
	if (obj is Item item)
	{
		// do something
	}
}
```

## EventToCommand를 이용한 더블클릭 처리

### 1. System.Windows.Interactivity 추가

```bash
Install-Package System.Windows.Interactivity.WPF
```

### 2. XAML에 네임스페이스 추가

```xml
xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
xmlns:cmd="clr-namespace:GalaSoft.MvvmLight.Command;assembly=GalaSoft.MvvmLight.Extras.WPF4"
```

### 3. ListView에 EventToCommand 추가

```xml
<ListView ItemsSource="{Binding Items}">
	<i:Interaction.Triggers>
		<i:EventTrigger EventName="MouseDoubleClick">
			<cmd:EventToCommand Command="{Binding DoubleClickCommand}" PassEventArgsToCommand="True"/>
		</i:EventTrigger>
	</i:Interaction.Triggers>
</ListView>
```

### 4. ViewModel에 DoubleClickCommand 추가

```csharp
public RelayCommand<MouseButtonEventArgs> DoubleClickCommand { get; }

public MainViewModel()
{
	DoubleClickCommand = new RelayCommand<MouseButtonEventArgs>(DoubleClick);
}

private void DoubleClick(MouseButtonEventArgs e)
{
	if (e.OriginalSource is ListViewItem item)
	{
		if (item.DataContext is Item data)
		{
			// do something
		}
	}
}
```

## 이벤트 핸들러를 이용한 더블클릭 처리

### 1. XAML에 이벤트 핸들러 추가

```xml
<ListView ItemsSource="{Binding Items}" MouseDoubleClick="ListView_MouseDoubleClick"/>
```

### 2. 이벤트 핸들러 작성

```csharp
private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
{
	if (e.OriginalSource is ListViewItem item)
	{
		if (item.DataContext is Item data)
		{
			// do something
		}
	}
}
```


### references

* https://kaki104.tistory.com/847
* https://blog.naver.com/vactorman/221064433289