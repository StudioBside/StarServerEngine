namespace Cs.HttpClient
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Cs.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class HttpResponseMessageExt
    {
        public static async Task<T?> GetContentAs<T>(this HttpResponseMessage self)
        {
            try
            {
                string responseBody = await self.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody) ?? throw new Exception($"deserialize failed. data:{responseBody}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return default;
            }
        }

        public static async Task<T?> GetContentAs<T>(this HttpResponseMessage self, Func<JObject, T> factory)
        {
            try
            {
                string responseBody = await ReadContentString(self.Content);
                var jObject = JsonConvert.DeserializeObject<JObject>(responseBody) ?? throw new Exception($"deserialize failed. data:{responseBody}");
                return factory(jObject);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return default;
            }
        }

        public static async Task<JObject?> GetContent(this HttpResponseMessage self)
        {
            try
            {
                string responseBody = await self.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JObject>(responseBody) ?? throw new Exception($"deserialize failed. data:{responseBody}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return default;
            }
        }

        //// -------------------------------------------------------------------
       
        private static async Task<string> ReadContentString(HttpContent content)
        {
            if (string.IsNullOrEmpty(content.Headers.ContentType?.CharSet) == false)
            {
                var encodingName = content.Headers.ContentType.CharSet;
                if (encodingName != "utf-8")
                {
                    var encoding = Encoding.GetEncoding(encodingName);
                    var stream = await content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream, encoding);
                    return reader.ReadToEnd();
                }
            }

            return await content.ReadAsStringAsync();
        }
    }
}
