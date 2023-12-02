![](https://raw.githubusercontent.com/StudioBside/.github/main/Images/logo_horizontal.png)

![.NET](https://github.com/StudioBside/StarServerEngine/actions/workflows/dotnet.yml/badge.svg) ![publish nuget](https://github.com/StudioBside/StarServerEngine/actions/workflows/publish-nuget.yml/badge.svg)

# Cs.Math

This is a project that gathers mathematical logics. It includes a BasisPoint type to represent probabilities, random functions, an implementation for random selection (lottery), and partial implementations for 2D geometry.

RatioLottery: Logic that performs random selection based on the weight of candidates in the list.
RateLottery: Logic that performs random selection based on the probability of candidates in the list (there might be losing outcomes depending on the settings).
BasisPoint: Represents a basis point type, ranging from 0 to 10000.
 
---

수학적인 로직들을 모아둔 프로젝트 입니다.
확률을 표현하기 위한 만분율 타입(BasisPoint), 랜덤 함수, 랜덤 뽑기용 구현제(lottery) 및 2d geometry 일부 구현 등을 포함하고 있습니다.

* `RatioLottery` : 후보 리스트의 가중치를 기반으로 랜덤 선정을 수행하는 로직.
* `RateLottery` : 후보 리스트의 확률을 기반으로 랜덤 선정. (설정에 따라 꽝이 나올 수 있다)
* `BasisPoint` : 만분율 타입. 0 ~ 10000 사이의 값을 가짐.

## Contact

mailto: github@studiobside.com
