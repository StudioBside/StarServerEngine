namespace Shared.Templet;

using System;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;

public sealed class TempletContainer<T> where T : ITemplet
{
    private static readonly TempletContainer<T> Instance = Singleton<TempletContainer<T>>.Instance;

    private readonly Dictionary<int, T> data = new();
    private readonly Dictionary<string, T> strData = new();

    public static IEnumerable<T> Values => Instance.data.Values;

    public static void Load(string fileName, Func<JToken, T?> factory)
    {
        Load(fileName, factory, strKeySelector: null);
    }

    public static void Load(string fileName, Func<JToken, T?> factory, Func<T, string>? strKeySelector)
    {
        var json = JsonUtil.Load(fileName);
        if (json["Data"] is not JArray jArray)
        {
            Log.ErrorAndExit($"[TempletContainer] file loading failed. fileName:{fileName} type:{typeof(T).Name}");
            return;
        }

        foreach (JToken token in jArray)
        {
            var templet = factory(token);
            if (templet is null)
            {
                continue;
            }

            Instance.data.Add(templet.Key, templet);
        }

        if (strKeySelector is not null)
        {
            foreach (var data in Values)
            {
                var strKey = strKeySelector.Invoke(data);
                Instance.strData.Add(strKey, data);
            }
        }
    }

    public static void Join()
    {
        foreach (var data in Values)
        {
            data.Join();
        }
    }

    public static void Validate()
    {
        foreach (var data in Values)
        {
            data.Validate();
        }
    }
}
