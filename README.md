![](https://raw.githubusercontent.com/StudioBside/.github/main/Images/logo_horizontal.png)

![.NET](https://github.com/StudioBside/Cs.Logging/actions/workflows/dotnet.yml/badge.svg) ![publish nuget](https://github.com/StudioBside/StarServerEngine/actions/workflows/publish-nuget.yml/badge.svg)

# StarServerEngine

This is the game server engine code for a new project at Studio Biside. Not all engine code will be disclosed; only basic content useful for side project development will be shared.

스튜디오비사이드의 신규 프로젝트의 게임서버 엔진 코드입니다. 
모든 엔진코드를 공개하지는 않습니다. side project 개발에 쓰일만한 기본적인 내용들만 공개됩니다.

![](https://raw.githubusercontent.com/StudioBside/StarServerEngine/main/Document/Images/project-list.png)

As of the current moment, the engine layer consists of approximately 20 projects. These projects are actively under development and are planned to undergo significant improvements in the future.

현재 시점 기준 엔진 레이어는 약 20여개의 프로젝트로 이루어져 있습니다. 프로젝트는 활발하게 개발 진행중이며 앞으로도 많은 개선을 이루어 나갈 예정입니다. 

Please join us as a companion in the journey of creating unprecedented joy in the world together!

세상에 없던 즐거움을 만들어 나가는 여정을 함께할 우리의 동료가 되어주세요!

---

## Modules by functionality

* [Cs.Logging](./Engine/Cs.Logging/README.md)
* [Cs.Core](./Engine/Cs.Core/README.md)
* [Cs.Math](./Engine/Cs.Math/README.md)
* [Cs.HttpClient](./Engine/Cs.HttpClient/README.md)
* Cs.Dynamic
* Cs.Slack
* Cs.Exception
* Cs.Perforce

## Tools

Several internal development tools listed below will be made public. They do not contain project-specific content, so they can be used directly, but there are no plans to distribute separately built executable files.

다음의 몇가지 내부 개발용 툴이 공개됩니다. 프로젝트에 종속적인 내용이 없어 바로 사용도 가능하지만, 빌드한 실행파일을 별도로 배포할 예정은 없습니다.

* [JsonMigrator](https://github.com/StudioBside/StarServerEngine/tree/main/Tool/JsonMigrator)
* [JsonSchemaValidator](https://github.com/StudioBside/StarServerEngine/tree/main/Tool/JsonSchemaValidator)

## Installation

download from [nuget.org](https://www.nuget.org/packages?q=studiobside)
```
dotnet add package Cs.Logging
dotnet add package Cs.Core
...
```

### License

This project is licensed under the MIT License. For details, see the [License File](LICENSE).

### Contact

mailto: github@studiobside.com
