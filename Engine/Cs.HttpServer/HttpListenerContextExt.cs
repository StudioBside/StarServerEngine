namespace Cs.HttpServer;

using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json;

public static class HttpListenerContextExt
{
    public static bool GetRequestBodyRaw(this HttpListenerContext context, [NotNullWhen(true)] out string? output)
    {
        string rawData = new StreamReader(context.Request.InputStream).ReadToEnd();
        if (string.IsNullOrEmpty(rawData))
        {
            Log.Warn($"invalid request body:{rawData}");
            output = default;
            return false;
        }

        output = rawData;
        return true;
    }

    public static bool RequestFormBodyToCollection(this HttpListenerContext context, [NotNullWhen(true)] out NameValueCollection? output)
    {
        string rawData = new StreamReader(context.Request.InputStream).ReadToEnd();
        if (string.IsNullOrEmpty(rawData))
        {
            output = null;
            Log.Warn($"invalid request body:{rawData}");
            return false;
        }

        string decoded = HttpUtility.HtmlDecode(rawData);
        output = HttpUtility.ParseQueryString(decoded);
        return true;
    }

    [return: MaybeNull]
    public static T GetRequest<T>(this HttpListenerContext context)
    {
        if (GetRequestBodyRaw(context, out string? output) == false)
        {
            return default;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(output);
        }
        catch (Exception ex)
        {
            Log.Error($"body to json failed. payload:{output} exception:{ex.Message}");
            return default;
        }
    }

    public static void SetResponseHtml(this HttpListenerContext context, string html)
    {
        var response = context.Response;
        response.ContentType = "text/html";
        response.ContentEncoding = Encoding.UTF8;
        var buffer = Encoding.UTF8.GetBytes(html);
        response.ContentLength64 = buffer.Length;

        try
        {
            using var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            response.StatusCode = 200;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            response.StatusCode = 500;
        }
    }

    public static void SetLowCaseResponse(this HttpListenerContext context, HttpStatusCode statusCode, object json)
    {
        byte[] buffer = JsonUtil.ToLowCaseJsonBuffer(json);
        context.SetResponseJson(buffer, statusCode);
    }

    public static void SetResponseJson(this HttpListenerContext context, HttpStatusCode statusCode, object json)
    {
        byte[] buffer = JsonUtil.ToJsonBuffer(json);
        context.SetResponseJson(buffer, statusCode);
    }

    public static void SetResponseJson(this HttpListenerContext context, string jsonText, HttpStatusCode statusCode)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(jsonText);
        context.SetResponseJson(buffer, statusCode);
    }

    public static void SetResponseJson(this HttpListenerContext context, byte[] buffer, HttpStatusCode statusCode)
    {
        var response = context.Response;

        try
        {
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.ContentLength64 = buffer.Length;

            using var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            response.StatusCode = (int)statusCode;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            response.StatusCode = 500;
        }
    }

    public static void SetResponseJsonError(this HttpListenerContext context, HttpStatusCode statusCode, string message)
    {
        byte[] buffer = JsonUtil.ToLowCaseJsonBuffer(new { Error = message });

        context.SetResponseJson(buffer, statusCode);
    }

    public static void SetResponseXml(this HttpListenerContext context, string xml)
    {
        var response = context.Response;

        try
        {
            response.ContentType = "text/xml";
            response.ContentEncoding = Encoding.UTF8;
            response.StatusCode = 200;
            var buffer = Encoding.UTF8.GetBytes(xml);
            response.ContentLength64 = buffer.Length;

            using var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            response.StatusCode = 200;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            response.StatusCode = 500;
        }
    }

    public static async Task SetResponseFileAsync(this HttpListenerContext context, FileStream stream, string mimeType)
    {
        var response = context.Response;
        response.ContentType = mimeType;
        response.ContentLength64 = stream.Length;
        response.StatusCode = (int)HttpStatusCode.OK;

        using (stream)
        using (var outputStream = response.OutputStream)
        {
            await stream.CopyToAsync(outputStream);
        }
    }

    public static void RedirectTo(this HttpListenerContext context, string url)
    {
        var response = context.Response;
        string fullUrl = $"http://{context.Request.UserHostName}{url}";
        response.Redirect(fullUrl);
        response.Close();
    }

    public static int GetInt32FromQueryString(this HttpListenerContext context, string key)
    {
        var value = context.Request.QueryString[key];
        if (value == null)
        {
            return 0;
        }

        if (int.TryParse(value, out int result) == false)
        {
            return 0;
        }

        return result;
    }

    public static long GetInt64FromQueryString(this HttpListenerContext context, string key)
    {
        var value = context.Request.QueryString[key];
        if (value == null)
        {
            return 0;
        }

        if (long.TryParse(value, out long result) == false)
        {
            return 0;
        }

        return result;
    }

    public static string GetStringFromQueryString(this HttpListenerContext context, string key)
    {
        return context.Request.QueryString[key] ?? string.Empty;
    }

    public static bool HasValueInQueryString(this HttpListenerContext context, string key)
    {
        return context.Request.QueryString[key] is not null;
    }
}
