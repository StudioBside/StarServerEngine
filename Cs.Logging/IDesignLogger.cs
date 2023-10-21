namespace Cs.Logging
{
    using System.Runtime.CompilerServices;

    public interface IDesignLogger
    {
        IDesignLogger WriteHead(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
        IDesignLogger WriteLine(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
        IDesignLogger WriteLine();
    }
}
