namespace Cs.Core.Util
{
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class TagBuilder
    {
        public static string Build(string file, int line)
        {
            return string.Intern($"{Path.GetFileName(file)}:{line.ToString()}");
        }
    }
}
