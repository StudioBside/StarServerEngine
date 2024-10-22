namespace Cs.Antlr
{
    using System.Collections.Generic;
    using System.IO;
    using Antlr4.StringTemplate;
    using Cs.Core;
    using Cs.Logging;

    public sealed class StringTemplateFactory
    {
        private readonly Dictionary<string, TemplateGroup> cache = new();
        private string filePath = string.Empty;

        public static StringTemplateFactory Instance => Singleton<StringTemplateFactory>.Instance;

        public bool Initialize(string path)
        {
            // template 파일 경로 유효성 확인
            if (Directory.Exists(path) == false)
            {
                Log.Error($"template input path do not exist:{path}");
                return false;
            }

            this.filePath = path;
            return true;
        }

        public Template CreateFromFile(string fileName, string name)
        {
            if (this.cache.TryGetValue(fileName, out var templetGroup) == false)
            {
                string groupFilePath = Path.Combine(this.filePath, fileName);
                groupFilePath = Path.GetFullPath(groupFilePath);
                templetGroup = new TemplateGroupFile(groupFilePath);
                this.cache.Add(fileName, templetGroup);
            }

            return templetGroup.GetInstanceOf(name);
        }

        public void CreateFromString(string sourceName, string text)
        {
            if (this.cache.ContainsKey(sourceName))
            {
                return;
            }

            var templetGroup = new TemplateGroupString(sourceName, text);
            this.cache.Add(sourceName, templetGroup);
        }

        public Template? GetTemplet(string sourceName, string name)
        {
            if (this.cache.TryGetValue(sourceName, out var templetGroup) == false)
            {
                return null;
            }

            return templetGroup.GetInstanceOf(name);
        }
    }
}
