namespace Excel2Json.ToHotswapTemplet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Antlr4.StringTemplate;

    using Cs.Antlr;
    using Cs.Logging;
    using Cs.Perforce;

    using Excel2Json.Binding;
    using Excel2Json.Functions;

    using static Excel2Json.Enums;

    internal sealed class ExtractHotswap : IExtract
    {
        private readonly List<Source> sources;

        public ExtractHotswap(
            string outputFile,
            List<Source> sources,
            string keyColumn,
            IReadOnlyList<Column> columns,
            IReadOnlyList<UniquenessRecord> uniquenesses)
        {
            this.OutputFile = outputFile;
            this.sources = sources;
            this.KeyColumn = keyColumn;
            this.Columns = columns;
            this.Uniquenesses = uniquenesses;
        }

        public string OutputFile { get; }
        public FileOutputDirection FileOutDirection => FileOutputDirection.Server;
        public IReadOnlyList<Source> Sources => this.sources;
        public string KeyColumn { get; private set; }
        public IReadOnlyList<Column> Columns { get; set; } = Array.Empty<Column>();
        public IReadOnlyList<UniquenessRecord> Uniquenesses { get; }
        public string DebugName => $"[{this.OutputFile}]";

        public string EntityClassName 
        { 
            get
            {
                string className = $"_{this.OutputFile.Replace("_TEMPLET", string.Empty).ToLower()}";
                className = Regex.Replace(className, "_[a-z]", e => e.Value.ToUpper()[1..]);
                return $"{className}Entity";
            }
        }

        public static Template CreateEntityTemplate()
        {
            return StringTemplateFactory.Instance.Create("Template_hotswapEntity.stg", "writeFile");
        }

        public static Template CreateTextTemplate()
        {
            return StringTemplateFactory.Instance.Create("Template_hotswapJson.stg", "writeFile");
        }

        public bool Initialize()
        {
            foreach (var source in this.sources)
            {
                if (source.LoadExcel(this) == false)
                {
                    return false;
                }
            }

            foreach (var uniquenessRecord in this.Uniquenesses)
            {
                if (uniquenessRecord.Initialize(this) == false)
                {
                    return false;
                }

                var checker = new UniquenessChecker(uniquenessRecord);
                foreach (var source in this.sources)
                {
                    if (checker.CheckUniqueness(source) == false)
                    {
                        return false;
                    }
                }
            }

            foreach (var column in this.Columns)
            {
                column.Initialize(this);
                if (column.Validate() == false)
                {
                    return false;
                }
            }

            var keyColumn = this.Columns.FirstOrDefault(e => e.Name == this.KeyColumn);
            if (keyColumn == null)
            {
                return false;
            }

            bool isDataTypeIntager = keyColumn.DataType switch
            {
                DataType.Int8 => true,
                DataType.Int16 => true,
                DataType.Int32 => true,
                DataType.Int64 => true,
                DataType.Uint8 => true,
                DataType.Uint16 => true,
                DataType.Uint32 => true,
                DataType.Uint64 => true,
                _ => false,
            };

            if (isDataTypeIntager == false)
            {
                // 일단 이렇게 처리하고 문제가 생기면 에러로 처리
                this.KeyColumn = $"{this.KeyColumn}.GetHashCode()";
            }

            return true;
        }

        public string GetFinalTextOutput(FileOutputDirection fileOutDir)
        {
            var config = Config.Instance;
            switch (fileOutDir)
            {
                case FileOutputDirection.Server:
                    return config.Path.ServerTextOutput;

                case FileOutputDirection.Client:
                    return config.Path.ClientTextOutput;

                case FileOutputDirection.Tool:
                    return config.Path.ToolTextOutput;

                default:
                    throw new Exception($"invalid outputDirection:{fileOutDir}");
            }
        }

        public string GetFinalBinOutput(FileOutputDirection fileOutDir)
        {
            var config = Config.Instance;
            switch (fileOutDir)
            {
                case FileOutputDirection.Server:

                    return config.Path.ServerBinOutput;

                case FileOutputDirection.Client:
                    return config.Path.ClientBinOutput;

                case FileOutputDirection.Tool:
                    return config.Path.ToolBinOutput;

                default:
                    throw new Exception($"invalid outputDirection:{fileOutDir}");
            }
        }

        public void WriteEntityFile(P4Commander p4Commander)
        {
            var template = CreateEntityTemplate();
            template.Add("model", this);

            string fullFilePath = this.BuildEntityFilePath();
            string fileName = Path.GetFileName(fullFilePath);
            if (File.Exists(fullFilePath))
            {
                File.SetAttributes(fullFilePath, FileAttributes.Normal);
            }

            using (var sw = new StreamWriter(fullFilePath, append: false, Encoding.UTF8))
            {
                sw.WriteLine(template.Render());
            }

            if (p4Commander.CheckIfOpened(fullFilePath))
            {
                return;
            }

            if (p4Commander.CheckIfChanged(fullFilePath, out bool changed) == false)
            {
                ErrorContainer.Add($"{this.DebugName} 변경여부 확인 실패. fileName:{fileName}");
                return;
            }

            if (changed == false)
            {
                return;
            }

            if (p4Commander.OpenForEdit(fullFilePath, out string p4Output) == false)
            {
                ErrorContainer.Add($"{this.DebugName} p4 edit 실패. p4Output:{p4Output}");
                return;
            }

            return;
        }

        bool IExtract.HasColumn(string columnName) => this.Columns.Any(e => e.Name == columnName);
        bool IExtract.GetColumn(string name, [MaybeNullWhen(false)] out Column result)
        {
            result = this.Columns.FirstOrDefault(e => e.Name == name);
            return result != null;
        }

        IEnumerable<Column> IExtract.GetAllColumns() => this.Columns;
        bool IExtract.HasSourceFrom(IReadOnlySet<string> targetExcelFiles)
        {
            // 인자로 받은 엑셀파일은 파일 이름만 들어있고, 멤버로 가진 sources는 상대경로를 갖고있다.
            foreach (var source in this.sources)
            {
                if (targetExcelFiles.Any(source.ExcelFile.Contains))
                {
                    return true;
                }
            }

            return false;
        }

        void IExtract.AddSystemColumn(SystemColumn systemColumn)
        {
            var newColumns = new List<Column>
            {
                new Column
                {
                    Name = systemColumn.ToString(),
                    DataType = DataType.String,
                    Nullable = false,
                },
            };

            newColumns.AddRange(this.Columns);
            this.Columns = newColumns;
        }

        //// ------------------------------------------------------------------------------------

        private string BuildEntityFilePath()
        {
            var config = Config.Instance;
            var outputPath = config.Path.HotswapEntityOutput;

            return Path.GetFullPath(Path.Combine(outputPath, $"{this.EntityClassName}{config.CSharpOutputExtension}"));
        }
    }
}
