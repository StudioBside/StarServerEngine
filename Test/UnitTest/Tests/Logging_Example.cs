namespace Cs.Logging.Test;

using Cs.Logging;
using Cs.Logging.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class Logging_Example
{
    [TestMethod]
    public void 기본_사용법()
    {
        // 초기화를 하지 않고 사용하면 NullLogProvider가 사용되어, 아무런 출력도 발생하지 않습니다. 
        // 사용할 provider의 객체와, 출력할 로그 레벨을 지정해줍니다.
        Log.Initialize(new ConsoleLogProvider(), LogLevelConfig.All);
        
        // 로그 메세지를 출력하는 인터페이스는 아래의 5종 입니다. 
        Log.Info("정보 출력을 위한 메세지. 디버깅 목적이 아니기 때문에 개발빌드, 릴리즈 빌드 모두에서 출력해야 할 정보를 찍는다.");
        Log.Debug("개발자가 디버깅을 위해 출력하는 메세지. 개발빌드에서만 출력된다. 디버깅 작업이 끝났으면 정리해야 한다.");
        Log.Warn("정상적이지 않은 경우이지만, 프로그램의 실행에는 문제가 없는 경우에 출력하는 메세지.");
        Log.Error("프로그램의 실행에 문제가 있는 수준의 오류. 진행하던 로직은 중단되어야 한다.");
        // Log.ErrorAndExit("프로그램의 실행에 치명적인 오류. 프로그램을 종료한다.");
    }
    
    [TestMethod]
    public void Provider_임시변환()
    {
        // 초기화를 여러번 호출하는 것은 크게 문제가 되지 않습니다만, 프로세스가 멀티 스레드에서 돌고 있다던지 할 땐 주의가 필요합니다. 
        Log.Initialize(NullLogProvider.Instance, LogLevelConfig.All);
        
        Log.Debug("provider가 null이어서, 메세지는 출력되지 않습니다.");
        
        using (var switcher = Log.SwitchProvider(new ConsoleLogProvider()))
        {
            Log.Debug("특정 구간에서만 잠시 provider를 변경하고 싶을 때 SwitchProvider를 사용합니다.");
        }
        
        Log.Debug("SwitchProvider를 벗어나면, 다시 NullLogProvider가 사용됩니다.");
    }
}