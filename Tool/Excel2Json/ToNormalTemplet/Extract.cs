namespace Excel2Json.ToNormalTemplet;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Cs.Logging;

using Excel2Json;
using Excel2Json.Binding;

using Excel2Json.Functions;

using static Excel2Json.Enums;

internal sealed class Extract : IExtract
{
    private readonly List<Source> sources;

    public Extract(
        string outputFile,
        string outputGroupBy,
        FileOutputDirection fileOutDirection,
        bool excludeToolOutput,
        List<Source> sources,
        BindRoot bindRoot,
        IReadOnlyList<UniquenessRecord> uniquenesses,
        CustomOutputPath customOutputPath,
        DuplicationCleanerRecord duplicationCleaner)
    {
        this.OutputFile = outputFile;
        this.OutputGroupBy = outputGroupBy;
        this.FileOutDirection = fileOutDirection;
        this.ExcludeToolOutput = excludeToolOutput;
        this.sources = sources;
        this.BindRoot = bindRoot;
        this.Uniquenesses = uniquenesses;
        this.CustomOutputPath = customOutputPath;
        this.DuplicationCleaner = duplicationCleaner;
    }

    public string OutputFile { get; }
    public string OutputGroupBy { get; }
    public FileOutputDirection FileOutDirection { get; }
    public bool ExcludeToolOutput { get; }
    public IReadOnlyList<Source> Sources => this.sources;
    public BindRoot BindRoot { get; }
    public IReadOnlyList<UniquenessRecord> Uniquenesses { get; }
    public CustomOutputPath CustomOutputPath { get; }
    public DuplicationCleanerRecord DuplicationCleaner { get; }
    public string DebugName => $"[{this.OutputFile}]";

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

        if (this.DuplicationCleaner is not null)
        {
            if (this.DuplicationCleaner.Initialize(this) == false)
            {
                return false;
            }
        }

        if (this.BindRoot.Initialize(this) == false)
        {
            return false;
        }

        if (string.IsNullOrEmpty(this.OutputGroupBy) == false)
        {
            if (this.BindRoot.GetColumn(this.OutputGroupBy, out var column) == false)
            {
                Log.Error($"{this.DebugName} groupBy로 지정한 컬럼이 유효하지 않습니다. columnName:{this.OutputGroupBy}");
                return false;
            }

            if (column.Nullable)
            {
                Log.Error($"{this.DebugName} groupBy로 지정한 컬럼은 nullable 일 수 없습니다. columnName:{this.OutputGroupBy}");
                return false;
            }
        }

        if (this.CustomOutputPath != null)
        {
            if (this.CustomOutputPath.Validate() == false)
            {
                Log.Error($"{this.DebugName} 별도 지정한 출력 경로가 유효하지 않습니다.");
                return false;
            }
        }

        return true;
    }

    public string GetFinalTextOutput(FileOutputDirection fileOutDir)
    {
        var config = Config.Instance;
        switch (fileOutDir)
        {
            case FileOutputDirection.Server:
                if (string.IsNullOrEmpty(this.CustomOutputPath?.ServerTextOutput) == false)
                {
                    return this.CustomOutputPath.ServerTextOutput;
                }

                return config.Path.ServerTextOutput;

            case FileOutputDirection.Client:
                if (string.IsNullOrEmpty(this.CustomOutputPath?.ClientTextOutput) == false)
                {
                    return this.CustomOutputPath.ClientTextOutput;
                }

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
                if (string.IsNullOrEmpty(this.CustomOutputPath?.ServerBinOutput) == false)
                {
                    return this.CustomOutputPath.ServerBinOutput;
                }

                return config.Path.ServerBinOutput;

            case FileOutputDirection.Client:
                if (string.IsNullOrEmpty(this.CustomOutputPath?.ClientBinOutput) == false)
                {
                    return this.CustomOutputPath.ClientBinOutput;
                }

                return config.Path.ClientBinOutput;

            case FileOutputDirection.Tool:
                return config.Path.ToolBinOutput;

            default:
                throw new Exception($"invalid outputDirection:{fileOutDir}");
        }
    }

    bool IExtract.HasColumn(string columnName) => this.BindRoot.HasColumn(columnName);
    bool IExtract.GetColumn(string name, [MaybeNullWhen(false)] out Column result) => this.BindRoot.GetColumn(name, out result);
    IEnumerable<Column> IExtract.GetAllColumns() => this.BindRoot.GetAllColumns();
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
        this.BindRoot.AddSystemColumn(systemColumn);
    }

    public void AdjustSource(IEnumerable<string> targetExcelFiles)
    {
        // 출력 파일 그룹 설정이 없으면 처리할 일 없음.
        if (string.IsNullOrEmpty(this.OutputGroupBy))
        {
            return;
        }

        // 출력 파일 그룹 설정이 있다면 꼭 Extrace 안의 모든 source가 같이 추출될 필요가 없다. 
        // 열려있는 엑셀 파일에 해당하지 않는 source들은 삭제해서 추출 작업에서 제외시킨다. 
        var validSources = new HashSet<string>();
        foreach (var source in this.sources)
        {
            if (targetExcelFiles.Any(source.ExcelFile.Contains))
            {
                validSources.Add(source.DebugName);
            }
        }

        if (this.sources.Count == validSources.Count)
        {
            return;
        }

        int prevCount = this.sources.Count;
        this.sources.RemoveAll(source =>
        {
            if (validSources.Contains(source.DebugName))
            {
                return false;
            }

            Log.Info($"[RemoveSource] source:{source.DebugName}");
            return true;
        });

        Log.Info($"[RemoveSource] extract:{this.DebugName} sourceCount:{prevCount} -> {this.sources.Count}");
    }
}
