![](https://raw.githubusercontent.com/StudioBside/.github/main/Images/logo_horizontal.png)

![.NET](https://github.com/StudioBside/Cs.Logging/actions/workflows/dotnet.yml/badge.svg) ![publish nuget](https://github.com/StudioBside/StarServerEngine/actions/workflows/publish-nuget.yml/badge.svg)

# Cs.Antlr

`Antlr4.StringTemplate`를 사용하는 모듈입니다. 

현재는 간단하게 `Antlr4.StringTemplet.TempletGroup` 타입의 인스턴스를 생성하고 캐싱해두는 기능만을 제공합니다.

아래의 예시와 같이 사용합니다.

## Getting Started

```csharp
if (StringTemplateFactory.Instance.Initialize(config.Path.TextTemplate) == false)
{
    Log.Error($"TextTemplate initialize failed.");
    return false;
}

...

// create template from file
var template = StringTemplateFactory.Instance.CreateFromFile("Template_enum.stg", "writeFile");

// or create templet from string
StringTempletFactory.Instance.CreateFromStgring("sourceName", "templateSourceBody...");
templet = StringTempletFactory.Instance.GetTemplet("sourceName", "writeFile");

templet.Add("model", this);

using (var sw = new StreamWriter(outputFilePath, append: false, Encoding.UTF8))
{
    sw.WriteLine(template.Render());
}
```

### Prerequisites

- .net SDK 8.0

### Installation

download from [nuget.org](https://www.nuget.org/packages/Cs.Antlr/)

```
dotnet add package Cs.Antlr
```

## License

This project is licensed under the MIT License. For details, see the [License File](../../LICENSE).

## Contact

mailto: github@studiobside.com