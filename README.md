![](https://raw.githubusercontent.com/StudioBside/.github/main/Images/logo_horizontal.png)

![.NET](https://github.com/StudioBside/Cs.Logging/actions/workflows/dotnet.yml/badge.svg) ![push to nuget.org](https://github.com/StudioBside/Cs.Logging/actions/workflows/nuget.yml/badge.svg?event=push)

# Cs.Logging

Log interface for gameserver development.

The main purpose is to define an interface for writing log messages, and the actual log recording part requires users to connect by creating a `LogProvider` directly.

It provides console or file logging output by default but does not guarantee performance or thread safety.

---

사내에서 사용하는 게임서버의 로그 작성 인터페이스 입니다. 

로그를 쓰는 곳의 인터페이스를 정의하는 것이 주 목적입니다. 로그 메세지를 어디로 어떻게 쓸것인지는 사용하는 쪽에서 직접 `LogProvider`를 만들어 연결해 주어야 합니다.

기본적으로 콘솔이나 파일로 출력하는 기능을 제공하지만 성능이나 스레드 안전성 등을 보장하지 않습니다.

## Getting Started

### Prerequisites

- .net SDK 7.0

### Installation

download from [nuget.org](https://www.nuget.org/packages/Cs.Logging/)
```
dotnet add package Cs.Logging
```

### Usage

```Csharp
using Cs.Logging;
using Cs.Logging.Providers;

Log.Initialize(new ConsoleLogProvider(), LogLevelConfig.All);

Log.Info("디버깅 목적이 아닌 데이터. 서비스 환경에서도 남겨야 할 메시지들.");
Log.Debug("디버깅을 위한 임시 메세지. 개발빌드에서만 사용하고 디버깅 작업이 끝나면 정리해야 한다.");
Log.Warn("정상적이지 않은 경우이지만, 프로그램의 실행에는 문제가 없는 경우에 출력하는 메세지.");
Log.Error("프로그램의 실행에 문제가 있는 수준의 오류. 진행하던 로직은 중단되어야 한다.");
Log.ErrorAndExit("프로그램의 실행에 치명적인 오류. 프로그램을 종료한다.");
```

### License

This project is licensed under the MIT License. For details, see the [License File](LICENSE).

### Contact

mailto: github@studiobside.com

### Additional Information

* [ExampleTest.cs](./Cs.Logging.Test/Tests/ExampleTest.cs)
* [OutputTest.cs](./Cs.Logging.Test/Tests/OutputTest.cs)