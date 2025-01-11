namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model.Detail;
using CutEditor.Model.L10n;
using CutEditor.ViewModel.Detail;
using CutEditor.ViewModel.L10n;
using CutEditor.ViewModel.L10n.Strategies;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.Model.Enums;
using static StringStorage.Enums;

public sealed class VmL10n : VmPageBase,
    IFileDropHandler
{
    private readonly ObservableCollection<string> logMessages = new();
    private readonly IServiceProvider services;
    private string name = string.Empty;
    private string textFileName = string.Empty;
    private string? importFilePath;
    private bool hasEnglish;
    private bool hasJapanese;
    private bool hasChineseSimplified;
    private bool isSuccessful;
    private string importResult = string.Empty;
    private L10nType? loadingType;
    private IL10nStrategy strategy = new CutsceneNormalStrategy();

    public VmL10n(IServiceProvider services)
    {
        this.services = services;
        this.ApplyDataCommand = new RelayCommand(this.OnApplyData, () => this.LoadingType != null);
    }

    public IServiceProvider Services => this.services;
    public IRelayCommand ApplyDataCommand { get; }
    public IEnumerable<IL10nMapping> Mappings => this.strategy.Mappings;
    public IList<string> LogMessages => this.logMessages;
    public string? ImportFileName => Path.GetFileName(this.importFilePath);
    public string? ImportFilePath
    {
        get => this.importFilePath;
        set => this.SetProperty(ref this.importFilePath, value);
    }

    public bool HasEnglish
    {
        get => this.hasEnglish;
        set => this.SetProperty(ref this.hasEnglish, value);
    }

    public bool HasJapanese
    {
        get => this.hasJapanese;
        set => this.SetProperty(ref this.hasJapanese, value);
    }

    public bool HasChineseSimplified
    {
        get => this.hasChineseSimplified;
        set => this.SetProperty(ref this.hasChineseSimplified, value);
    }

    public string Name
    {
        get => this.name;
        set => this.SetProperty(ref this.name, value);
    }

    public string TextFileName
    {
        get => this.textFileName;
        set => this.SetProperty(ref this.textFileName, value);
    }

    public int StatCountNormal => this.strategy.Statistics[(int)L10nMappingState.Normal];
    public int StatCountMissingOrigin => this.strategy.Statistics[(int)L10nMappingState.MissingOrigin];
    public int StatCountMissingImported => this.strategy.Statistics[(int)L10nMappingState.MissingImported];
    public int StatCountTextChanged => this.strategy.Statistics[(int)L10nMappingState.TextChanged];

    public bool IsSuccessful
    {
        get => this.isSuccessful;
        set => this.SetProperty(ref this.isSuccessful, value);
    }

    public string ImportResult
    {
        get => this.importResult;
        set => this.SetProperty(ref this.importResult, value);
    }

    public L10nType? LoadingType
    {
        get => this.loadingType;
        set => this.SetProperty(ref this.loadingType, value);
    }

    public L10nSourceType L10nSourcetype => this.strategy.SourceType;

    void IFileDropHandler.HandleDroppedFiles(string[] files)
    {
        foreach (var file in files)
        {
            this.WriteLog($"파일 드롭: {file}");
        }

        if (files.Length == 1)
        {
            this.ProcessSingleFileDrop(files[0]);
            return;
        }

        this.ProcessMultiFileDrop(files);
    }

    public void WriteLog(string message)
    {
        var formatted = $"[{DateTime.Now:HH:mm:ss}] {message}";
        this.logMessages.Add(formatted);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.ImportFilePath):
                this.OnPropertyChanged(nameof(this.ImportFileName));
                break;

            case nameof(this.LoadingType):
                this.ApplyDataCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void OnApplyData()
    {
        if (this.loadingType == null || this.loadingType == L10nType.Kor)
        {
            this.WriteLog($"번역을 적용할 언어타입 지정이 올바르지 않습니다. loadingType:{this.loadingType}");
            return;
        }

        if (this.strategy.SourceCount == 0)
        {
            this.WriteLog("적용할 데이터가 없습니다.");
            return;
        }

        int changedCount;

        changedCount = 0;
        foreach (var mapping in this.Mappings)
        {
            if (mapping.ApplyData(this.loadingType.Value))
            {
                ++changedCount;
            }
        }

        if (changedCount == 0)
        {
            this.WriteLog("적용할 변경사항이 없습니다.");
            return;
        }

        if (this.strategy.SaveToFile(this.Name) == false)
        {
            this.WriteLog("적용에 실패했습니다.");
            return;
        }

        this.WriteLog($"번역 적용 완료. 대상 언어:{this.loadingType.Value} 변경된 데이터 {changedCount}개.");
    }

    private bool LoadOriginData(string name, L10nSourceType sourceType)
    {
        var newStrategy = sourceType switch
        {
            L10nSourceType.CutsceneNormal => new CutsceneNormalStrategy(),
            _ => throw new NotSupportedException($"지원하지 않는 타입입니다. type:{sourceType}"),
        };

        bool success = newStrategy.LoadOriginData(name, this);
        if (success == false)
        {
            this.WriteLog("원본 데이터 로딩에 실패했습니다.");
            return false;
        }

        this.strategy = newStrategy;
        this.OnPropertyChanged(nameof(this.L10nSourcetype));
        this.OnPropertyChanged(nameof(this.Mappings));

        this.Name = name;
        this.Title = this.Name;
        this.TextFileName = CutFileIo.GetTextFileName(this.Name);
        return true;
    }

    private void ProcessSingleFileDrop(string file)
    {
        var nameOnly = Path.GetFileNameWithoutExtension(file);

        // 파일 이름으로 데이터 타입을 판별.
        var sourceType = nameOnly switch
        {
            _ when nameOnly.StartsWith("SYSTEM_STRING_") => L10nSourceType.SystemString,
            _ when nameOnly.Contains("SHORTEN_") => L10nSourceType.CutsceneShorten,
            _ => L10nSourceType.CutsceneNormal,
        };

        this.WriteLog($"소스 데이터 타입:{sourceType}");

        // 이미 로딩한 원본 데이터가 있고 타입이 일치하면 원본 데이터 재사용.
        if (this.L10nSourcetype == sourceType && this.Name == nameOnly)
        {
            this.WriteLog("로딩한 원본 데이터가 재사용됩니다.");
        }
        else
        {
            this.WriteLog("새로운 원본 데이터 로딩.");
            if (this.LoadOriginData(nameOnly, sourceType) == false)
            {
                return;
            }
        }

        this.ImportFile(file);
    }

    private void ProcessMultiFileDrop(IReadOnlyList<string> files)
    {
    }

    private bool ImportFile(string fileFullPath)
    {
        var nameOnly = Path.GetFileNameWithoutExtension(fileFullPath);
        this.WriteLog($"번역데이터 파일 로딩: {nameOnly}");
        this.ImportFilePath = null;

        this.LoadingType = null;

        var importedHeaders = new HashSet<string>();
        this.strategy.ImportFile(fileFullPath, this, importedHeaders);

        this.HasEnglish = importedHeaders.Contains("English");
        this.HasJapanese = importedHeaders.Contains("Japanese");
        this.HasChineseSimplified = importedHeaders.Contains("ChineseSimplified");

        this.ImportFilePath = fileFullPath;
        this.IsSuccessful = this.strategy.SourceCount == this.strategy.Statistics[(int)L10nMappingState.Normal];
        this.ImportResult = this.IsSuccessful
            ? "모든 데이터의 uid 및 텍스트가 일치합니다."
            : "데이터 불일치. 확인이 필요합니다.";

        this.WriteLog(this.ImportResult);

        this.OnPropertyChanged(nameof(this.StatCountNormal));
        this.OnPropertyChanged(nameof(this.StatCountMissingOrigin));
        this.OnPropertyChanged(nameof(this.StatCountMissingImported));
        this.OnPropertyChanged(nameof(this.StatCountTextChanged));

        return true;
    }
}