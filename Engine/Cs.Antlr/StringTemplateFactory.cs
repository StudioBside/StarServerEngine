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
        private string filePath;

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

        public Template Create(string fileName, string name)
        {
            if (this.cache.TryGetValue(fileName, out var templetGroup) == false)
            {
                string groupFilePath = Path.Combine(this.filePath, fileName);
                groupFilePath = Path.GetFullPath(groupFilePath);
                templetGroup = new TemplateGroupFile(groupFilePath);
            }

            return templetGroup.GetInstanceOf(name);
        }
    }
}
