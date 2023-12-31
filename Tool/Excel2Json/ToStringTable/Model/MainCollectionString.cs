namespace Excel2Json.ToStringTable.Model;

using System.Data;
using System.Text;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Perforce;
using Cs.Protocol;
using Excel2Json;
using Excel2Json.Functions;
using Excel2Json.ToNormalTemplet.Model;
using Excel2Json.ToStringTable;
using static Excel2Json.Enums;

// 스트링을 출력하는 json 파일 하나에 해당하는 타입.
// 클라 한쪽으로만 출력한다. stable에 따른 구분이 있다.
internal sealed class MainCollectionString
{
    public static readonly string ResultTableHeader;
    private readonly Dictionary<string, SingleStringData> dataListByValue = new();

    static MainCollectionString()
    {
        ResultTableHeader = string.Join(
           " | ",
           string.Empty,
           "Output File".PadRight(35),
           "FileType".PadRight(8),
           "Stable".PadRight(6),
           "bef.Size".PadRight(8),
           "aft.Size".PadRight(8),
           "Result".PadRight(15),
           string.Empty);
    }

    private MainCollectionString(ExtractString extract, FileOutputDirection fileOutDir, bool isStable)
    {
        this.Extract = extract;
        this.FileOutDirection = fileOutDir;
        this.IsStable = isStable;
    }

    public ExtractString Extract { get; }
    public FileOutputDirection FileOutDirection { get; }
    public bool IsStable { get; }
    #region Antlr Interface
    public string Namespace => this.Extract.NameSpace;
    public IEnumerable<SingleStringData> DataList => this.dataListByValue.Values;
    #endregion
    internal string DebugName => $"[{this.Extract.DebugName}] stable:{this.IsStable}";

    public static MainCollectionString? Create(ExtractString extract, FileOutputDirection fileOutDir, bool isStable)
    {
        var result = new MainCollectionString(extract, fileOutDir, isStable);
        foreach (var source in extract.Sources)
        {
            if (isStable == false && source.HasStableColumn == false)
            {
                continue; // unstable 데이터를 뽑아야 하는데 시트에 아예 IsStable 지정 컬럼이 없는 경우 - 패스.
            }

            foreach (DataRow dataRow in source.DataTable.Rows)
            {
                if (source.HasStableColumn)
                {
                    var value = dataRow.GetString(SystemColumn.IsStable);
                    if (MainCollection.ParseStableColumnValue(value, out bool dataIsStable) == false)
                    {
                        ErrorContainer.Add($"{extract.DebugName} {SystemColumn.IsStable}컬럼 값이 올바르지 않습니다.");
                        return null;
                    }

                    if (isStable != dataIsStable)
                    {
                        continue;
                    }
                }

                var stringId = dataRow.GetString(StringTableColumn.StringId.ToString());
                if (string.IsNullOrEmpty(stringId))
                {
                    ErrorContainer.Add($"{extract.DebugName} stringId를 읽을 수 없습니다");
                    return null;
                }

                var stringValue = dataRow.GetString(StringTableColumn.StringValue.ToString());
                if (string.IsNullOrEmpty(stringValue))
                {
                    ErrorContainer.Add($"{extract.DebugName} stringValue 읽을 수 없습니다. stringId:{stringId}");
                    return null;
                }

                if (result.dataListByValue.TryGetValue(stringValue, out var stringData) == false)
                {
                    stringData = new SingleStringData(stringValue);
                    result.dataListByValue.Add(stringValue, stringData);
                }

                stringData.AddKey(stringId);
            }
        }

        if (result.dataListByValue.Any() == false)
        {
            return null;
        }

        return result;
    }

    public FileWritingResult WriteTextFile(in P4Commander p4Commander)
    {
        var template = ExtractString.CreateAntlrTemplate();
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

        long sourceSizeSum = this.Extract.Sources.Sum(e => e.FileSize);
        FileUtil.GetFileSize(fullFilePath, out var fileSize);
        var logHelper = new ResultLogHelper(this, this.Extract.OutputFile, "text", sourceSizeSum, fileSize);
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

    public void WriteBinFile(P4Commander p4Commander)
    {
        string textFilePath = this.BuildTextOutputFilePath();
        if (File.Exists(textFilePath) == false)
        {
            ErrorContainer.Add($"{this.DebugName} text 파일을 찾을 수 없습니다. textFilePath:{textFilePath}");
            return;
        }

        string binFilePath = this.BuildBinOutputFilePath();
        FileUtil.GetFileSize(textFilePath, out var textFileSize);

        string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        if (Text2Binary.ConvertJsonBinary(textFilePath, tempFilePath) == false)
        {
            ErrorContainer.Add($"{this.DebugName} text -> binary 변환 실패");
            return;
        }

        FileSystem.GuaranteePath(binFilePath);

        string binFileName = Path.GetFileName(binFilePath);
        if (File.Exists(binFilePath))
        {
            File.SetAttributes(binFilePath, FileAttributes.Normal);
        }

        //File.Move(tempFilePath, binFilePath, overwrite: true);
        //var (BeforeBytes, AfterBytes) = (0, 0);
        var (beforeBytes, afterBytes) = Lz4Util.CompressNew(tempFilePath, binFilePath);
        var logHelper = new ResultLogHelper(this, this.Extract.OutputFile, "bytes", textFileSize, afterBytes);
        if (p4Commander.CheckIfOpened(binFilePath))
        {
            logHelper.WriteLog(FileWritingResult.AlreadyOpened);
            return;
        }

        if (p4Commander.CheckIfChanged(binFilePath, out bool changed) == false)
        {
            ErrorContainer.Add($"{this.DebugName} 변경여부 확인 실패. fileName:{binFileName}");
            return;
        }

        if (changed == false)
        {
            logHelper.WriteLog(FileWritingResult.NotChanged);
            return;
        }

        if (p4Commander.OpenForEdit(binFilePath, out string p4Output) == false)
        {
            ErrorContainer.Add($"{this.DebugName} p4 edit 실패. p4Output:{p4Output}");
            return;
        }

        logHelper.WriteLog(FileWritingResult.Changed);
    }

    //// ------------------------------------------------------------------------------------

    private string BuildTextOutputFilePath()
    {
        var prefix = MainCollection.TryAddFileTypePrefix(this.FileOutDirection, this.Extract.OutputFile);

        var config = Config.Instance;
        var outputPath = this.Extract.GetFinalTextOutput(this.FileOutDirection);
        string fileName = this.IsStable
            ? $"{prefix}{config.TextOutputExtension}"
            : $"{prefix}.unstable{config.TextOutputExtension}";
        return Path.GetFullPath(Path.Combine(outputPath, fileName));
    }

    private string BuildBinOutputFilePath()
    {
        var prefix = MainCollection.TryAddFileTypePrefix(this.FileOutDirection, this.Extract.OutputFile);

        var config = Config.Instance;
        var outputPath = this.Extract.GetFinalBinOutput(this.FileOutDirection);
        string fileName = this.IsStable
            ? $"{prefix}{config.BinOutputExtension}"
            : $"{prefix}.unstable{config.BinOutputExtension}";
        return Path.GetFullPath(Path.Combine(outputPath, fileName));
    }

    private readonly struct ResultLogHelper
    {
        private readonly MainCollectionString owner;
        private readonly string fileName;
        private readonly string fileType;
        private readonly long sizeBefore;
        private readonly long sizeAfter;

        public ResultLogHelper(MainCollectionString owner, string fileName, string fileType, long sizeBefore, long sizeAfter)
        {
            this.owner = owner;
            this.fileName = fileName;
            this.fileType = fileType;
            this.sizeBefore = sizeBefore;
            this.sizeAfter = sizeAfter;
        }

        public FileWritingResult WriteLog(FileWritingResult result)
        {
            var stableMark = this.owner.IsStable ? "O" : "X";
            Log.Info($" | {this.fileName,-35} | {this.fileType,8} | {stableMark,6} | {this.sizeBefore.ToByteFormat(),8} | {this.sizeAfter.ToByteFormat(),8} | {result,15} |");
            return result;
        }
    }
}
