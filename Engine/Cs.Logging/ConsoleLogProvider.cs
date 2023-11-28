namespace Cs.Logging
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    /// <summary>
    /// 별도 초기화가 없을 땐 기본 동작으로 콘솔만 출력한다.
    /// </summary>
    internal sealed class ConsoleLogProvider : ILogProvider
    {
        public void Debug(string message) => Console.WriteLine(message);
        public void DebugBold(string message) => Console.WriteLine(message);
        public void Info(string message) => Console.WriteLine(message);
        public void Warn(string message) => Console.WriteLine(message);

        [DoesNotReturn]
        public void ErrorAndExit(string message)
        {
            Console.WriteLine(message);
            Process.GetCurrentProcess().Kill();
            throw new Exception(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public string BuildTag(string file, int line)
        {
            return string.Intern($"{Path.GetFileName(file)}:{line}");
        }
    }
}
