# <img src="./Document/Images/icon.png" width=50> Cs.Logging

StubioBside gameserver logging interface

The main purpose is to define an interface for writing log messages, and the actual log recording part requires users to connect by creating a `LogProvider` directly.

it provides console or file logging output by default but does not guarantee performance or thread safety.

사내에서 사용하는 게임서버의 로그 작성 인터페이스 입니다. 

로그를 쓰는 곳의 인터페이스를 정의하는 것이 주 목적입니다. 로그 메세지를 어디로 어떻게 쓸것인지는 사용하는 쪽에서 직접 `LogProvider`를 만들어 연결해 주어야 합니다.

기본적으로 콘솔이나 파일로 출력하는 기능을 제공하지만 성능이나 스레드 안전성 등을 보장하지 않습니다.