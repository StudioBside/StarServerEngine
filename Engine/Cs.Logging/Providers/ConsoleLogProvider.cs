namespace Cs.Logging.Providers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Cs.Logging.Detail;
    using static Cs.Logging.Log;

    /// <summary>
    /// 별도 초기화가 없을 땐 기본 동작으로 콘솔만 출력한다.
    /// </summary>
    public sealed class ConsoleLogProvider : ILogProvider
    {
        public void Debug(string message) => ConsoleWriter.PutLog(LogLevel.Debug, message);
        public void DebugBold(string message) => ConsoleWriter.PutLog(LogLevel.Debug, message, ConsoleColor.Cyan);
        public void Info(string message) => ConsoleWriter.PutLog(LogLevel.Info, message);
        public void Warn(string message) => ConsoleWriter.PutLog(LogLevel.Warn, message);
        public void Error(string message) => ConsoleWriter.PutLog(LogLevel.Error, message);

        [DoesNotReturn]
        public void ErrorAndExit(string message)
        {
            this.Error(message);
            Process.GetCurrentProcess().Kill();
            throw new Exception(message);
        }

        public string BuildTag(string file, int line)
        {
            return string.Intern($"{Path.GetFileName(file)}:{line}");
        }
    }
}
