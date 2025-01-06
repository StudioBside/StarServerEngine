namespace StringStorage.Translation;

using System.Diagnostics.CodeAnalysis;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json;
using static StringStorage.Enums;

public sealed class SingleTextSet
{
    public static SingleTextSet Empty { get; } = new SingleTextSet();

    public string ValueEng { get; set; } = string.Empty;
    public string ValueChnS { get; set; } = string.Empty;
    public string ValueChnT { get; set; } = string.Empty;
    public string ValueJpn { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static bool TryLoad(string jsonString, [MaybeNullWhen(false)] out SingleTextSet result)
    {
        try
        {
            var converted = JsonConvert.DeserializeObject<SingleTextSet>(jsonString);
            if (converted is null)
            {
                Log.Error($"[ContentsEntity] try loading failed. jsonString:{jsonString}");
                result = default;
                return false;
            }

            result = converted;
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"[ContentsEntity] try loading failed. jsonString:{jsonString} message:{ex.Message}");
            result = default;
            return false;
        }
    }

    public static SingleTextSet? TryCreate(string jsonString)
    {
        if (TryLoad(jsonString, out var result))
        {
            return result;
        }

        return null;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, GlobalSetting.JsonSerializerSettings);
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this, GlobalSetting.JsonSerializerSettings);
    }

    public string GetValueString(L10nType type)
    {
        return type switch
        {
            L10nType.Eng => this.ValueEng,
            L10nType.ChnS => this.ValueChnS,
            L10nType.ChnT => this.ValueChnT,
            L10nType.Jpn => this.ValueJpn,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    public void SetValueString(L10nType type, string value)
    {
        switch (type)
        {
            case L10nType.Eng:
                this.ValueEng = value;
                break;
            case L10nType.ChnS:
                this.ValueChnS = value;
                break;
            case L10nType.ChnT:
                this.ValueChnT = value;
                break;
            case L10nType.Jpn:
                this.ValueJpn = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        this.UpdatedAt = ServiceTime.Recent;
    }
}
