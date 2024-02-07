namespace Cs.Logging.DesignLoggers
{
    using System.Runtime.CompilerServices;

    using Cs.Logging;

    public sealed class LoggerToNull : IDesignLogger
    {
        public static LoggerToNull Instance { get; } = new LoggerToNull();

        public int Indent { get; set; }
        public string IndentString { get; set; } = string.Empty;

        public IDesignLogger WriteHead(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) => this;
        public IDesignLogger WriteLine(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) => this;
        public IDesignLogger WriteLine() => this;
    }
}
