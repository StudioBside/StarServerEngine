namespace Cs.Logging
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public sealed class NullLogProvider : ILogProvider
    {
        private NullLogProvider()
        {
        }

        public static ILogProvider Instance { get; } = new NullLogProvider();

        public void Debug(string message)
        {
        }

        public void DebugBold(string message)
        {
        }
        
        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }

        [DoesNotReturn]
        public void ErrorAndExit(string message)
        {
            Console.WriteLine(message);
            Process.GetCurrentProcess().Kill();
            throw new Exception(message);
        }

        public void Error(string message)
        {
        }

        public string BuildTag(string file, int line)
        {
            return string.Intern($"{Path.GetFileName(file)}:{line}");
        }
    }
}
