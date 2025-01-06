namespace StringStorage;

using Newtonsoft.Json;

internal static class GlobalSetting
{
    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
    };
}
