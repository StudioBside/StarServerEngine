![.NET](https://github.com/StudioBside/Cs.Logging/actions/workflows/dotnet.yml/badge.svg) ![publish nuget](https://github.com/StudioBside/StarServerEngine/actions/workflows/publish-nuget.yml/badge.svg)

# Cs.Core

This is a project that gathers commonly used basic functionalities across various projects. It deals with handling time (DateTime), utilizing xml and json formatted data, zip compression, executing external processes, and includes miscellaneous functionalities.

It also encompasses several data structures not provided by System.Collection.Generic.

* `CircularBuffer`: A thread-safe circular buffer for multi-threaded access.
* `LruCache`: A cache container using LinkedList<T> (not thread-safe for multi-threaded access).
* `PriorityQueue`: A priority queue using the Heap data structure (though now [natively provided from .NET 6 onwards](https://learn.microsoft.com/ko-kr/dotnet/api/system.collections.generic.priorityqueue-2)).

---

여러 프로젝트에서 범용적으로 쓰이는 기본 동작들을 모아둔 프로젝트 입니다. 
시간(DateTime)을 다루거나, xml, json 포맷 데이터 사용, zip 압축, 외부 process 실행 등 다양하고 소소한 소품들을 모아둡니다. 
`System.Collection.Generic`에서 기본 제공되지 않는 몇가지 자료구조도 포함하고 있습니다.

* `CircularBuffer` : 멀티 스레드 접근에 안전한 순환형 버퍼
* `LruCache` : LinkedList<T> 사용한 캐시 컨테이너. (멀티스레드 접근에 안전하지 않음)
* `PriorityQueue` : Heap 자료구조를 이용한 우선순위 큐. (하지만 이제는 [.net6부터 기본 제공](https://learn.microsoft.com/ko-kr/dotnet/api/system.collections.generic.priorityqueue-2)됩니다.)

## Getting Started

### Prerequisites

- .net SDK 8.0

### Installation

download from [nuget.org](https://www.nuget.org/packages/Cs.Core/)
```
dotnet add package Cs.Core
```

### License

This project is licensed under the MIT License. For details, see the [License File](../../LICENSE).

### Contact

mailto: github@studiobside.com