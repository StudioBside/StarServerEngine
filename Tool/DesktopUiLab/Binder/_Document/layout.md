## WPF Panel류 활용


### 컨트롤 배치시 '나머지 공간 모두 차지' 식의 설정을 하는 방법

- WPF에서는 Panel을 이용하여 컨트롤을 배치한다.
- Panel에는 여러 종류가 있는데, 각 Panel마다 배치 방식이 다르다.
- 그 중에서도 '나머지 공간 모두 차지' 식의 설정을 하는 방법을 알아보자.


### 1. Grid를 이용한 '나머지 공간 모두 차지' 식의 설정

- Grid는 행과 열로 구성되어 있다.
- 각 행과 열에는 각각의 크기를 지정할 수 있다.
- 이때 '*'를 이용하면 '나머지 공간 모두 차지' 식의 설정을 할 수 있다.
- 예를 들어, 다음과 같이 설정하면 첫 번째 행은 100, 두 번째 행은 200, 세 번째 행은 나머지 공간 모두 차지로 설정된다.

```xml
<Grid>
	<Grid.RowDefinitions>
		<RowDefinition Height="100"/>
		<RowDefinition Height="200"/>
		<RowDefinition Height="*"/>
	</Grid.RowDefinitions>
</Grid>
```

### 2. DockPanel을 이용한 '나머지 공간 모두 차지' 식의 설정

- DockPanel은 컨트롤을 상하좌우로 배치할 수 있다.
- 이때, '나머지 공간 모두 차지' 식의 설정을 하려면 LastChildFill 속성을 True로 설정하면 된다.
- 예를 들어, 다음과 같이 설정하면 첫 번째 컨트롤은 상단에, 두 번째 컨트롤은 하단에, 세 번째 컨트롤은 나머지 공간 모두 차지로 설정된다.

```xml
<DockPanel LastChildFill="True">
	<Button DockPanel.Dock="Top" Content="Top" HorizontalAlignment="Center" VerticalAlignment="Center"/>
	<Button DockPanel.Dock="Bottom" Content="Bottom" HorizontalAlignment="Center" VerticalAlignment="Center"/>
	<Button Content="*" HorizontalAlignment="Center" VerticalAlignment="Center"/>
</DockPanel>
```


### 3. StackPanel을 이용한 '나머지 공간 모두 차지' 식의 설정

- StackPanel은 컨트롤을 위아래로 배치할 수 있다.
- 이때, '나머지 공간 모두 차지' 식의 설정을 하려면 Orientation 속성을 Horizontal로 설정하고, HorizontalAlignment 속성을 Stretch로 설정하면 된다.
- 예를 들어, 다음과 같이 설정하면 첫 번째 컨트롤은 왼쪽에, 두 번째 컨트롤은 오른쪽에, 세 번째 컨트롤은 나머지 공간 모두 차지로 설정된다.

```xml
<StackPanel Orientation="Horizontal">
	<Button Content="Left" HorizontalAlignment="Stretch" VerticalAlignment="Center"/>
	<Button Content="Right" HorizontalAlignment="Stretch" VerticalAlignment="Center"/>
	<Button Content="*" HorizontalAlignment="Stretch" VerticalAlignment="Center"/>
</StackPanel>
```
