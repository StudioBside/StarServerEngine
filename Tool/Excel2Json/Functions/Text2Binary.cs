namespace Excel2Json.Functions
{
    using Cs.Core.Util;
    using Cs.Logging;

    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Linq;

    internal static class Text2Binary
    {
        public static bool ConvertJsonBinary(string textFilePath, string binFilePath)
        {
            if (JsonUtil.TryLoad<JToken>(textFilePath, out var jToken) == false)
            {
                ErrorContainer.Add($"텍스트 파일 로딩 실패. filePath:{textFilePath}");
                return false;
            }

            using var fileStream = new FileStream(binFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            using (var writer = new BsonDataWriter(fileStream))
            {
                jToken.WriteTo(writer);
            }

            return true;
        }
    }
}
