namespace CutEditor.Model.Detail;

using System;

internal static class OutputTransfer
{
    public static int? EliminateZeroInt(int source)
    {
        return source > 0 ? source : null;
    }

    public static bool? EliminateFalse(bool source)
    {
        return source ? source : null;
    }

    public static T? EliminateEnum<T>(T source, T defaultValue) where T : struct, Enum
    {
        return source.Equals(defaultValue) ? null : source;
    }
}
