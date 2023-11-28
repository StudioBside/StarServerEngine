namespace Cs.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    public interface IDesignLogger
    {
        int Indent { get; set; }
        string IndentString { get; set; }

        IDesignLogger WriteHead(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
        IDesignLogger WriteLine(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
        IDesignLogger WriteLine();

        public readonly struct Indentaion : IDisposable
        {
            private readonly IDesignLogger logger;

            public Indentaion(IDesignLogger logger)
            {
                this.logger = logger;
                this.logger.Indent++;
                this.logger.IndentString = string.Empty.PadRight(this.logger.Indent * 4, ' ');
            }

            public void Dispose()
            {
                this.logger.Indent--;
                this.logger.IndentString = string.Empty.PadRight(this.logger.Indent * 4, ' ');
                this.logger.WriteLine(); // 들여쓰기가 끝날 땐 한 칸 띄워준다.
            }
        }
    }
}
