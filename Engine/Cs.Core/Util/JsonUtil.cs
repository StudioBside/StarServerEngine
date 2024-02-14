namespace Cs.Core.Util
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Cs.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    public static class JsonUtil
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new LowercaseContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static T Load<T>(string fileName)
        {
            var body = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<T>(body) ?? throw new Exception($"deserialize failed. fileName:{fileName}");
        }

        public static bool WriteToFile<T>(string fileName, T data)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };

            try
            {
                string body = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(fileName, body);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[JsonUtil] write failed. fileName:{fileName} message:{ex.Message}");
                return false;
            }
        }

        public static bool TryLoad<T>(string fileName, [MaybeNullWhen(false)] out T result)
        {
            if (File.Exists(fileName) == false)
            {
                result = default;
                return false;
            }

            try
            {
                result = Load<T>(fileName);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[JsonUtil] try loading failed. fileName:{fileName} message:{ex.Message}");
                result = default;
                return false;
            }
        }

        public static string ToJson(this NameValueCollection self)
        {
            var dictionary = self.AllKeys.ToDictionary(key => key ?? string.Empty, key => self.Get(key));
            string body = JsonConvert.SerializeObject(dictionary, Formatting.Indented)
               .Replace('"', '\'');
            return $"\"{body}\"";
        }

        public static string ToJsonString(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static string ToLowerKeyJson(object target)
        {
            return JsonConvert.SerializeObject(target, JsonSerializerSettings);
        }

        public static byte[] ToJsonBuffer(object data)
        {
            string jsonText = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(jsonText);
        }

        public static byte[] ToLowCaseJsonBuffer(object data)
        {
            string jsonText = JsonConvert.SerializeObject(data, JsonSerializerSettings);
            return Encoding.UTF8.GetBytes(jsonText);
        }

        public static T ToObject<T>(JToken token, string key)
        {
            var subToken = token.SelectToken(key);
            if (subToken != null)
            {
                return subToken.ToObject<T>() ?? throw new JsonSerializationException(key);
            }

            throw new JsonSerializationException(key);
        }

        //// --------------------------------------------------------------

        private sealed class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
    }
}
