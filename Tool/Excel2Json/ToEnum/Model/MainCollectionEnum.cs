namespace Excel2Json.ToEnum.Model;

using System.Data;
using System.Text;
using Cs.Logging;
using Cs.Perforce;
using Excel2Json;
using Excel2Json.ToEnum;
using static Excel2Json.Enums;

// enum 정의한 .cs 파일 하나에 해당하는 타입.
// enum에는 stability와 groupBy 속성이 없다. direction만 툴과 클라 양쪽으로 출력한다.
internal sealed class MainCollectionEnum
{
    public static readonly string ResultTableHeader;
    private readonly Dictionary<string /*enumTypeName*/, EnumSetModel> enumSets = new();

    static MainCollectionEnum()
    {
        ResultTableHeader = string.Join(
           " | ",
           string.Empty,
           "Output File".PadRight(25),
           "Direction".PadRight(10),
           "#enumTypes".PadRight(10),
           "#enumValues".PadRight(11),
           "Result".PadRight(10),
           string.Empty);
    }

    private MainCollectionEnum(ExtractEnum extract, FileOutputDirection fileOutDir)
    {
        this.Extract = extract;
        this.FileOutDir = fileOutDir;
    }

    public ExtractEnum Extract { get; }
    public FileOutputDirection FileOutDir { get; }
    #region Antlr Interface
    public string Namespace => this.Extract.NameSpace;
    public IEnumerable<EnumSetModel> EnumSets => this.enumSets.Values;
    #endregion
    internal string DebugName => $"[{this.Extract.DebugName} {this.FileOutDir}]";

    public static MainCollectionEnum? Create(ExtractEnum extract, FileOutputDirection fileOutDir)
    {
        if (fileOutDir == FileOutputDirection.All)
        {
            ErrorContainer.Add($"파일 출력 방향은 All을 사용할 수 없음");
            return null;
        }

        if ((extract.FileOutDirection == FileOutputDirection.Server && fileOutDir == FileOutputDirection.Client)
            || (extract.FileOutDirection == FileOutputDirection.Client && fileOutDir == FileOutputDirection.Server))
        {
            return null;
        }

        var result = new MainCollectionEnum(extract, fileOutDir);
        foreach (DataRow dataRow in extract.Source.DataTable.Rows)
        {
            var enumTypeName = dataRow.GetString(ExcelEnumColumn.EnumTypeName);
            if (string.IsNullOrEmpty(enumTypeName))
            {
                ErrorContainer.Add($"{extract.DebugName} 유효하지 않은 enumType:{enumTypeName}");
                return null;
            }

            if (result.enumSets.TryGetValue(enumTypeName, out var enumSet) == false)
            {
                enumSet = new EnumSetModel(extract, enumTypeName);
                result.enumSets.Add(enumTypeName, enumSet);
            }

            enumSet.Load(dataRow);
        }

        if (result.enumSets.Any() == false)
        {
            return null;
        }

        return result;
    }

    public FileWritingResult WriteCsharpFile(in P4Commander p4Commander)
    {
        var template = ExtractEnum.CreateCsharpTemplate();
        template.Add("model", this);

        string fullFilePath = this.BuildTextOutputFilePath();
        string fileName = Path.GetFileName(fullFilePath);
        if (File.Exists(fullFilePath))
        {
            File.SetAttributes(fullFilePath, FileAttributes.Normal);
        }

        using (var sw = new StreamWriter(fullFilePath, append: false, Encoding.UTF8))
        {
            sw.WriteLine(template.Render());
        }

        long enumValuesCount = this.enumSets.Values.Sum(e => e.EnumElements.Count);
        var logHelper = new ResultLogHelper(this, this.enumSets.Count, enumValuesCount);
        if (p4Commander.CheckIfOpened(fullFilePath))
        {
            return logHelper.WriteLog(FileWritingResult.AlreadyOpened);
        }

        if (p4Commander.CheckIfChanged(fullFilePath, out bool changed) == false)
        {
            ErrorContainer.Add($"{this.DebugName} 변경여부 확인 실패. fileName:{fileName}");
            return FileWritingResult.Error;
        }

        if (changed == false)
        {
            return logHelper.WriteLog(FileWritingResult.NotChanged);
        }

        if (p4Commander.OpenForEdit(fullFilePath, out string p4Output) == false)
        {
            ErrorContainer.Add($"{this.DebugName} p4 edit 실패. p4Output:{p4Output}");
            return FileWritingResult.Error;
        }

        return logHelper.WriteLog(FileWritingResult.Changed);
    }

    //// ------------------------------------------------------------------------------------

    private string BuildTextOutputFilePath()
    {
        var config = Config.Instance;
        var outputPath = this.FileOutDir switch
        {
            //FileOutputDirection.Server => config.Path.ServerEnumOutput,
            FileOutputDirection.Client => config.Path.ClientEnumOutput,
            FileOutputDirection.Tool => config.Path.ToolEnumOutput,
            _ => throw new Exception($"invalid outputDirection:{this.FileOutDir}"),
        };

        string fileName = $"{this.Extract.OutputFile}{config.CSharpOutputExtension}";
        return Path.GetFullPath(Path.Combine(outputPath, fileName));
    }

    private readonly struct ResultLogHelper
    {
        private readonly MainCollectionEnum owner;
        private readonly long enumTypesCount;
        private readonly long enumValuesCount;

        public ResultLogHelper(MainCollectionEnum owner, long enumTypesCount, long enumValuesCount)
        {
            this.owner = owner;
            this.enumTypesCount = enumTypesCount;
            this.enumValuesCount = enumValuesCount;
        }

        public FileWritingResult WriteLog(FileWritingResult result)
        {
            Log.Info($" | {this.owner.Extract.OutputFile,-25} | {this.owner.FileOutDir,10} | {this.enumTypesCount,10} | {this.enumValuesCount,11} | {result,-10} |");
            return result;
        }
    }
}
