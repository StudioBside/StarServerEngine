namespace Cs.Logging.DesignLoggers
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Cs.Logging;

    public sealed class LoggerToMemory : IDesignLogger, IDisposable
    {
        private static readonly string MessageHeaderFence = string.Empty.PadRight(70, '-');
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly StringWriter writer;
      
        public LoggerToMemory()
        {
            this.writer = new StringWriter(this.stringBuilder);
        }

        public int Indent { get; set; }
        public string IndentString { get; set; } = string.Empty;

        public void Dispose()
        {
            this.writer.Dispose();
        }

        public override string ToString()
        {
            return this.stringBuilder.ToString();
        }

        public IDesignLogger WriteHead(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            this.writer.WriteLine($"{this.IndentString}{MessageHeaderFence}");
            this.writer.WriteLine($"{this.IndentString}[DesignLogHead] {message}");
            this.writer.WriteLine($"{this.IndentString}{MessageHeaderFence}");

            return this;
        }

        public IDesignLogger WriteLine(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            this.writer.WriteLine($"{this.IndentString}{message}");
            return this;
        }

        public IDesignLogger WriteLine()
        {
            this.writer.WriteLine();
            return this;
        }
    }
}
