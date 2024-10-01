namespace Cs.Core.Util;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Cs.Logging;

public static class JsonHelper
{
    public static JsonDocument LoadJsonc(string filePath)
    {
        var options = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        string jsonc = File.ReadAllText(filePath);
        return JsonDocument.Parse(jsonc, options);
    }

    public static bool TryLoadJsonc(string filePath, [MaybeNullWhen(false)] JsonDocument document)
    {
        try
        {
            document = LoadJsonc(filePath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return false;
        }
    }
}
