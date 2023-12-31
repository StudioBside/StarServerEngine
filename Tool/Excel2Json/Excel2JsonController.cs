namespace Excel2Json;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs.Antlr;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Messaging;
using Cs.Perforce;
using Cs.ServerEngine.Util;
using Excel2Json.Binding;
using Excel2Json.ToEnum;
using Excel2Json.ToEnum.Model;
using Excel2Json.ToHotswapTemplet;
using Excel2Json.ToHotswapTemplet.Model;
using Excel2Json.ToNormalTemplet;
using Excel2Json.ToNormalTemplet.Model;
using Excel2Json.ToStringTable;
using Excel2Json.ToStringTable.Model;
using static Excel2Json.Enums;

internal sealed class Excel2JsonController
{
    private readonly string[] args;
    private readonly List<Extract> extracts = new();
    private readonly List<ExtractEnum> extractEnums = new();
    private readonly List<ExtractString> extractStrings = new();
    private readonly List<ExtractHotswap> extractHotswap = new();
    private readonly HashSet<string> targetExcelFiles = new(); // p4 편집중인 엑셀만 추출할 때 추출 대상
    private readonly HashSet<string> targetBindingFiles = new(); // 외부에서 직접 명시한 포함(혹은 배체) 바인딩 파일.
    private readonly DateTime startAt = ServiceTime.Recent;
    private P4Commander p4Commander;
    private ExcelSelectOption excelSelectOption;

    public Excel2JsonController(string[] args)
    {
        this.args = args;
    }

    private enum ExcelSelectOption
    {
        All,
        P4Opened,
        WhiteList,
        BlackList,
    }

    public bool Initialize()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // https://github.com/ExcelDataReader/ExcelDataReader
        Console.OutputEncoding = Encoding.UTF8;

        Log.Initialize(new LogProvider("./", "Log.txt", writeTimeAndLevel: false), LogLevelConfig.All);
        Log.WriteFileLine = false;

        if (Config.Initiaize() == false)
        {
            return false;
        }

        if (P4Commander.TryCreate(out this.p4Commander) == false)
        {
            Log.Error($"p4 환경 정보 조회 실패");
            return false;
        }

        if (this.SelectExcelFiles() == false)
        {
            return false;
        }

        return true;
    }

    public bool Run()
    {
        switch (this.excelSelectOption)
        {
            case ExcelSelectOption.P4Opened:
                if (this.targetExcelFiles.Any() == false)
                {
                    Log.Info($"변환을 수행할 대상을 찾지 못했습니다. mode:{this.excelSelectOption}");
                    return true;
                }

                break;

            case ExcelSelectOption.BlackList:
            case ExcelSelectOption.WhiteList:
                if (this.targetBindingFiles.Any() == false)
                {
                    Log.Info($"변환을 수행할 대상을 찾지 못했습니다. mode:{this.excelSelectOption}");
                    return true;
                }

                break;
        }

        var config = Config.Instance;
        if (StringTemplateFactory.Instance.Initialize(config.Path.TextTemplate) == false)
        {
            Log.Error($"TextTemplate initialize failed.");
            return false;
        }

        if (this.LoadBindingRules() == false)
        {
            Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
            return false;
        }

        if (this.AdjustBindingRules() == false)
        {
            Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
            return false;
        }

        Log.Info(Log.BuildHead("Loading Excel"));
        Parallel.ForEach(this.extracts, extract =>
        {
            if (extract.Initialize() == false)
            {
                ErrorContainer.Add($"init extract failed. extract:{extract.DebugName}");
                return;
            }
        });

        Parallel.ForEach(this.extractEnums, extract =>
        {
            if (extract.Initialize() == false)
            {
                ErrorContainer.Add($"init enum-extract failed. extract:{extract.DebugName}");
                return;
            }
        });

        Parallel.ForEach(this.extractStrings, extract =>
        {
            if (extract.Initialize() == false)
            {
                ErrorContainer.Add($"init string-extract failed. extract:{extract.DebugName}");
                return;
            }
        });

        Parallel.ForEach(this.extractHotswap, extract =>
        {
            if (extract.Initialize() == false)
            {
                ErrorContainer.Add($"init hotswap-extract failed. extract:{extract.DebugName}");
                return;
            }
        });

        if (ErrorContainer.HasError)
        {
            Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
            return false;
        }

        Log.Info(Log.BuildHead("Extract & Write"));
        string headerPad = string.Empty.PadRight(MainCollection.ResultTableHeader.Length, '-');
        Log.Info(headerPad);
        Log.Info(MainCollection.ResultTableHeader);
        Log.Info(headerPad);

        // 일반 템플릿 데이터 : 텍스트 파일, 바이너리 파일 출력
        Parallel.ForEach(this.extracts, extract =>
        {
            var mainCollections = new[]
            {
                MainCollection.Create(extract, FileOutputDirection.Server, isStable: true),
                MainCollection.Create(extract, FileOutputDirection.Client, isStable: true),
                MainCollection.Create(extract, FileOutputDirection.Tool, isStable: true),
                MainCollection.Create(extract, FileOutputDirection.Server, isStable: false),
                MainCollection.Create(extract, FileOutputDirection.Client, isStable: false),
                MainCollection.Create(extract, FileOutputDirection.Tool, isStable: false),
            };

            Parallel.ForEach(mainCollections, collection =>
            {
                if (collection is null)
                {
                    return;
                }

                if (collection.NeedToRemove)
                {
                    collection.Remove(this.p4Commander);
                    return;
                }

                if (collection.WriteTextFile(this.p4Commander) == false)
                {
                    ErrorContainer.Add($"{collection.DebugName} write textFile failed.");
                }

                collection.WriteBinFile(this.p4Commander);
            });
        });

        if (ErrorContainer.HasError)
        {
            Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
            return false;
        }

        Log.Info(headerPad);

        var dataVersion = MainCollectionHotswap.GetDataVersion();

        // 핫스왑 템플릿 데이터 : 텍스트 파일, 바이너리 출력
        Parallel.ForEach(this.extractHotswap, extract =>
        {
            var mainCollections = new[]
            {
                MainCollectionHotswap.Create(extract, FileOutputDirection.Server, isStable: true, dataVersion),
                MainCollectionHotswap.Create(extract, FileOutputDirection.Tool, isStable: true, dataVersion),
                MainCollectionHotswap.Create(extract, FileOutputDirection.Server, isStable: false, dataVersion),
                MainCollectionHotswap.Create(extract, FileOutputDirection.Tool, isStable: false, dataVersion),
            };

            extract.WriteEntityFile(this.p4Commander);

            if (ErrorContainer.HasError)
            {
                Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
                return;
            }

            Parallel.ForEach(mainCollections, collection =>
            {
                if (collection is null)
                {
                    return;
                }

                if (collection.NeedToRemove)
                {
                    collection.Remove(this.p4Commander);
                    return;
                }

                if (collection.WriteTextFile(this.p4Commander) == false)
                {
                    ErrorContainer.Add($"{collection.DebugName} write textFile failed.");
                }

                collection.WriteBinFile(this.p4Commander);
            });
        });

        if (ErrorContainer.HasError)
        {
            Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
            return false;
        }

        Log.Info(headerPad);

        if (this.extractStrings.Any())
        {
            headerPad = string.Empty.PadRight(MainCollectionString.ResultTableHeader.Length, '-');
            Log.Info(headerPad);
            Log.Info(MainCollectionString.ResultTableHeader);
            Log.Info(headerPad);

            // 스트링 테이블 데이터 : 텍스트 파일, 바이너리 파일 출력
            Parallel.ForEach(this.extractStrings, extract =>
            {
                var mainCollections = new[] 
                {
                    MainCollectionString.Create(extract, FileOutputDirection.Client, isStable: true),
                    MainCollectionString.Create(extract, FileOutputDirection.Tool, isStable: true),
                    MainCollectionString.Create(extract, FileOutputDirection.Client, isStable: false),
                    MainCollectionString.Create(extract, FileOutputDirection.Tool, isStable: false),
                };

                Parallel.ForEach(mainCollections, collection =>
                {
                    if (collection is null)
                    {
                        return;
                    }

                    var textWriteResult = collection.WriteTextFile(this.p4Commander);
                    if (textWriteResult == FileWritingResult.Error)
                    {
                        ErrorContainer.Add($"{collection.DebugName} write textFile failed.");
                    }

                    collection.WriteBinFile(this.p4Commander);
                });
            });

            if (ErrorContainer.HasError)
            {
                Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
                return false;
            }

            Log.Info(headerPad);
        }

        headerPad = string.Empty.PadRight(MainCollectionEnum.ResultTableHeader.Length, '-');
        Log.Info(headerPad);
        Log.Info(MainCollectionEnum.ResultTableHeader);
        Log.Info(headerPad);

        // enum 데이터 : cs 파일 출력
        Parallel.ForEach(this.extractEnums, extract =>
        {
            var mainCollections = new[] 
            {
                //MainCollectionEnum.Create(extract, FileOutputDirection.Server),
                MainCollectionEnum.Create(extract, FileOutputDirection.Client),
                MainCollectionEnum.Create(extract, FileOutputDirection.Tool),
            };

            foreach (var collection in mainCollections)
            {
                if (collection == null)
                {
                    continue;
                }

                var textWriteResult = collection.WriteCsharpFile(in this.p4Commander);
                if (textWriteResult == FileWritingResult.Error)
                {
                    Log.Error($"{collection.DebugName} write textFile failed.");
                    continue;
                }
            }
        });

        Log.Info(headerPad);

        var (textPathList, binPathList) = this.GatherOutputPathList();

        var cSharpPathList = new[]
        {
            //config.Path.ServerEnumOutput,
            config.Path.ClientEnumOutput,
            config.Path.ToolEnumOutput,

            // 핫스왑 엔티티 위치
            config.Path.HotswapEntityOutput,
        };

        if (ErrorContainer.HasError)
        {
            Log.Error($"{ErrorContainer.ErrorCount}개의 에러가 발생했습니다.");
            this.p4Commander.RevertAll(textPathList, config.TextOutputExtension, out _);
            this.p4Commander.RevertAll(binPathList, config.BinOutputExtension, out _);
            this.p4Commander.RevertAll(cSharpPathList, config.CSharpOutputExtension, out _);
            return false;
        }

        this.p4Commander.AddNewFiles(textPathList, config.TextOutputExtension);
        this.p4Commander.AddNewFiles(binPathList, config.BinOutputExtension);
        this.p4Commander.AddNewFiles(cSharpPathList, config.CSharpOutputExtension);

        // 일반적인 1회 실행이라면 발생하지 않지만, 기획자가 한 파일을 반복 작업하면 발생 가능하므로 처리해준다.
        this.p4Commander.RevertUnchnaged(textPathList, config.TextOutputExtension, out _);
        this.p4Commander.RevertUnchnaged(binPathList, config.BinOutputExtension, out _);
        this.p4Commander.RevertUnchnaged(cSharpPathList, config.CSharpOutputExtension, out _);

        this.DumpExcutionReport();
        return true;
    }

    //// --------------------------------------------------------------------

    private (string[] TextOutputs, string[] BinOutputs) GatherOutputPathList()
    {
        var config = Config.Instance;
        var textPathList = new List<string>
        {
            config.Path.ServerTextOutput,
            config.Path.ClientTextOutput,
            config.Path.ToolTextOutput,
        };
        var binPathList = new List<string>
        {
            config.Path.ServerBinOutput,
            config.Path.ClientBinOutput,
            config.Path.ToolBinOutput,
        };

        foreach (var data in this.extracts.Select(e => e.CustomOutputPath))
        {
            if (data == null)
            {
                continue;
            }

            textPathList.AddRange(data.GetTextOutputs());
            binPathList.AddRange(data.GetBinOutputs());
        }

        return (textPathList.ToArray(), binPathList.ToArray());
    }

    private bool LoadBindingRules()
    {
        Log.Info(Log.BuildHead("Loading BindingRule"));

        var config = Config.Instance;
        var info = new DirectoryInfo(config.Path.BindingRule);
        List<FileInfo> bindFiles = info.GetFiles("Bind*.jsonc", SearchOption.AllDirectories).ToList();

        if (ErrorContainer.HasError)
        {
            return false;
        }

        foreach (var bindFile in bindFiles)
        {
            if (JsonUtil.TryLoad<FileLoadingType>(bindFile.FullName, out var loadingType) == false)
            {
                Log.Error($"loading bindingRule failed. fileName:{bindFile.Name}");
                continue;
            }

            int validCount = 0;
            int totalCount = 0;
            if (loadingType.Extracts != null)
            {
                totalCount = loadingType.Extracts.Count;
                foreach (var extract in loadingType.Extracts)
                {
                    if (Filter(extract) == false)
                    {
                        continue;
                    }

                    this.extracts.Add(extract);
                    ++validCount;
                }
            }

            if (loadingType.ExtractEnums != null)
            {
                totalCount += loadingType.ExtractEnums.Count;
                foreach (var extract in loadingType.ExtractEnums)
                {
                    if (Filter(extract) == false)
                    {
                        continue;
                    }

                    this.extractEnums.Add(extract);
                    ++validCount;
                }
            }

            if (loadingType.ExtractStrings != null)
            {
                totalCount += loadingType.ExtractStrings.Count;
                foreach (var extract in loadingType.ExtractStrings)
                {
                    if (Filter(extract) == false)
                    {
                        continue;
                    }

                    this.extractStrings.Add(extract);
                    ++validCount;
                }
            }

            if (loadingType.ExtractHotswap != null)
            {
                totalCount += loadingType.ExtractHotswap.Count;
                foreach (var extract in loadingType.ExtractHotswap)
                {
                    if (Filter(extract) == false)
                    {
                        continue;
                    }

                    this.extractHotswap.Add(extract);
                    ++validCount;
                }
            }

            Log.Debug($"loading {bindFile.Name} ... ok. #extract:{validCount} / {totalCount}");
        }

        return ErrorContainer.HasError == false;

        bool Filter(IExtract extract)
        {
            if (this.excelSelectOption == ExcelSelectOption.All)
            {
                return true;
            }

            if (this.excelSelectOption == ExcelSelectOption.P4Opened)
            {
                return extract.HasSourceFrom(this.targetExcelFiles);
            }

            return true;
        }
    }

    private bool AdjustBindingRules()
    {
        Log.Info(Log.BuildHead("Adjust BindingRule"));
        foreach (var extract in this.extracts)
        {
            extract.AdjustSource(this.targetExcelFiles);
        }

        return true;
    }

    private bool SelectExcelFiles()
    {
        if (this.args.Any())
        {
            if (Enum.TryParse(this.args[0], ignoreCase: true, out this.excelSelectOption) == false)
            {
                Log.Error($"커맨드 인자값이 올바르지 않습니다. args[0]:{this.args[0]}");
                return false;
            }
        }

        Log.Info(Log.BuildHead("Select Excel"));
        Log.Info($"엑셀 선택방식: {this.excelSelectOption}");

        var config = Config.Instance;
        switch (this.excelSelectOption)
        {
            case ExcelSelectOption.All:
                foreach (var targetExcel in Directory.EnumerateFiles(config.Path.ExcelInput, "*.xlsx", SearchOption.AllDirectories))
                {
                    var fileName = Path.GetFileName(targetExcel);
                    this.targetExcelFiles.Add(fileName);
                }

                break;
            case ExcelSelectOption.P4Opened:
                var excelPath = config.Path.ExcelInput;
                var excelFiles = this.p4Commander.GetOpenedFiles(excelPath, ".xlsx", out var p4Output);
                if (excelFiles == null)
                {
                    Log.Error($"open된 excel 파일 목록 조회 실패. excelPath:{excelPath} p4Output:{p4Output}");
                    return false;
                }

                if (excelFiles.Any())
                {
                    foreach (var fullPath in excelFiles)
                    {
                        var fileName = Path.GetFileName(fullPath);
                        this.targetExcelFiles.Add(fileName);
                        Log.Debug($" - {fileName}");
                    }
                }

                break;
        }

        return true;
    }

    private void DumpExcutionReport()
    {
        Log.Info(Log.BuildHead("ExecutionReport"));

        var current = ServiceTime.Recent;
        var elapsed = current - this.startAt;
        Log.Info($"추출 시작 시각: {this.startAt.ToDefaultString()}");
        Log.Info($"추출 완료 시각: {current.ToDefaultString()}");
        Log.Info($"총 수행시간: {elapsed}");

        var allSources = this.extracts.SelectMany(e => e.Sources);

        int excelFileCount = allSources.GroupBy(e => e.ExcelFile).Count();
        int excelSheetCount = allSources.Count();
        Log.Info($"대상 엑셀 파일 수: {excelFileCount}");
        Log.Info($"대상 엑셀 시트 수: {excelSheetCount}");
    }

    private record struct FileLoadingType(
        List<Extract> Extracts,
        List<ExtractEnum> ExtractEnums,
        List<ExtractString> ExtractStrings,
        List<ExtractHotswap> ExtractHotswap);
}
