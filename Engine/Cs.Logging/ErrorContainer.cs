namespace Cs.Logging
{
    using System.Runtime.CompilerServices;

    public static class ErrorContainer
    {
        private static int errorCount;
   
        public static bool HasError => errorCount > 0;
        public static int ErrorCount => errorCount;

        public static void Add(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            Log.Error(message, file, line);
            ++errorCount;
        }

        public static void Validate()
        {
            if (errorCount > 0)
            {
                Log.ErrorAndExit($"[ErrorContainer] {errorCount}개의 오류 발생");
            }
        }

        public static void Clear()
        {
            // 서버 실행 도중 hotswap을 하기 전에 상태 초기화 하기 위한 용도.
            errorCount = 0;
        }
    }
}
