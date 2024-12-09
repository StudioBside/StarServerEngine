namespace Cs.Core.Util;

using System.Runtime.CompilerServices;

public readonly record struct CodeTag(string File, int Line)
{
    public static CodeTag Build([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        return new CodeTag(file, line);
    }
}
