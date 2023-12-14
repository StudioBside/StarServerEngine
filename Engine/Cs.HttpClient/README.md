![.NET](https://github.com/StudioBside/Cs.Logging/actions/workflows/dotnet.yml/badge.svg) ![publish nuget](https://github.com/StudioBside/StarServerEngine/actions/workflows/publish-nuget.yml/badge.svg)

# Cs.HttpClient

This project gathers logic related to handling HTTP requests required for using external REST APIs. In C#, there are several interfaces like HttpRequest, WebClient, and HttpClient for processing HTTP requests. Although HttpClient is the most current form, caution is necessary when using it.

Generally, for lightweight requests with low load, there are no issues. However, when multiple threads aim to handle a massive throughput momentarily, performance problems might arise. Below are links to documents related to this matter for further reference.
 
---

C#에서 외부 rest api를 사용하기 위해 필요한 http request 처리에 관한 로직들을 모았습니다. 
C#으로 http 요청을 처리하는 데에는 HttpRequest, WebClient, HttpClient등 많은 인터페이스가 있습니다. 가장 최종적인 형태는 HttpClient이지만, 사용시에 주의가 필요합니다. 
일반적으로 부하가 크지 않은 가벼운 요청의 경우는 아무런 문제가 없지만, 다수의 스레드가 순간적으로 대량의 throughput을 처리하고자 할 땐 성능 문제가 발생할 수 있습니다. 이와 관련된 문서들의 링크를 아래에 정리했으니 참고 바랍니다.

## Getting Started

### Prerequisites

- .net SDK 8.0

### Installation

download from [nuget.org](https://www.nuget.org/packages/Cs.HttpClient/)

```
dotnet add package Cs.HttpClient
```

### Usage
```csharp

```

## License

This project is licensed under the MIT License. For details, see the [License File](../../LICENSE).

## Contact

mailto: github@studiobside.com

## See Also

* https://makolyte.com/csharp-how-to-make-concurrent-requests-with-httpclient/
* https://hwanstory.kr/@kim-hwan/posts/DotNet-HttpConnectionPooling
* https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
* https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#httpclient-lifetime-management
* https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
