namespace CutEditor.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

public sealed class CutPreset
{
    private static readonly JsonSerializer JsonSerializer;
    private readonly JToken token;

    static CutPreset()
    {
        var setting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters =
                    [
                        new StringEnumConverter(),
            ],
        };
        
        JsonSerializer = JsonSerializer.Create(setting);
    }

    public CutPreset(Cut cut)
    {
        this.token = JToken.FromObject(cut.ToOutputJsonType(), JsonSerializer);
    }

    public Cut CreateCut(long newUid)
    {
        this.token["Uid"] = newUid;
        return new Cut(this.token, debugName: "CutPreset");
    }
}
