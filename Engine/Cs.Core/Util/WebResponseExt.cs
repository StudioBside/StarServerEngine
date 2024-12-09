namespace Cs.Core.Util;

using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class WebResponseExt
{
    public static JObject DeserializeBody(this WebResponse response)
    {
        string? responseBody = null;
        using (var streamReader = new StreamReader(response.GetResponseStream()))
        {
            responseBody = streamReader.ReadToEnd().Trim();
        }

        return JsonConvert.DeserializeObject<JObject>(responseBody) ?? throw new Exception($"deserialize failed. data:{responseBody}");
    }

    public static T DeserializeBody<T>(this WebResponse response)
    {
        string? responseBody = null;
        using (var streamReader = new StreamReader(response.GetResponseStream()))
        {
            responseBody = streamReader.ReadToEnd().Trim();
        }

        return JsonConvert.DeserializeObject<T>(responseBody) ?? throw new Exception($"deserialize failed. data:{responseBody}");
    }
}
