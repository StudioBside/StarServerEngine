## todo

* nuget 설정 필요
* github actions 설정. 

```shell
dotnet build -c Release
dotnet pack -c Release
dotnet nuget push ./Cs.Logging/bin/Release/Cs.Logging.0.0.1.nupkg --source https://api.nuget.org/v3/index.json --api-key $NUGET_API_KEY 
```

## done

* namespace 사용 스타일 정리
* 기본 동작용 파일로그 추가
* visual studio에서는 제대로 빌드되지 않는 문제
* test 프로젝트 추가
* readme 파일 적기