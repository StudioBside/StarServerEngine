namespace Cs.Core.Util;

using System.Reflection;

public static class AssemblyExt
{
    public static string GetResourceString(this Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return string.Empty;
        }

        using var reader = new System.IO.StreamReader(stream);
        return reader.ReadToEnd();
    }
}
