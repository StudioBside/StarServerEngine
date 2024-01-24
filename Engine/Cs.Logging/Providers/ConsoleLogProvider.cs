namespace Cs.Logging.Providers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using static Cs.Logging.Log;

    /// <summary>
    /// 별도 초기화가 없을 땐 기본 동작으로 콘솔만 출력한다.
    /// </summary>
    public sealed class ConsoleLogProvider : ILogProvider
    {
        public void Debug(string message) => this.PutLog(LogLevel.Debug, message);
        public void DebugBold(string message) => this.PutLog(LogLevel.Debug, message, ConsoleColor.Cyan);
        public void Info(string message) => this.PutLog(LogLevel.Info, message);
        public void Warn(string message) => this.PutLog(LogLevel.Warn, message);

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

        private void PutLog(LogLevel level, string message, ConsoleColor? color = null)
        {
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }
            else
            {
                switch (level)
                {
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogLevel.Warn:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
