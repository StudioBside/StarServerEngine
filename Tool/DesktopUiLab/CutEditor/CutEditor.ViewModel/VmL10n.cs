namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Detail;
using CutEditor.Model.Interfaces;
using CutEditor.Model.L10n;
using CutEditor.ViewModel.Detail;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.Model.Enums;

public sealed class VmL10n : VmPageBase
{
    private readonly Dictionary<long/*uid*/, Cut> originCuts = new();
    private readonly Dictionary<string/*uidStr*/, L10nMapping> mappings = new();
    private readonly ObservableCollection<CutOutputExcelFormat> importedCuts = new();
    private readonly List<CutOutputExcelFormat> importedMappings = new();
    private readonly HashSet<string> importedHeaders = new();
    private readonly ObservableCollection<string> logMessages = new();
    private readonly IServiceProvider services;
    private readonly int[] mappingStat = new int[EnumUtil<L10nMappingType>.Count];
    private string? importFilePath;
    private bool hasEnglish;
    private bool hasJapanese;
    private bool hasChineseSimplified;

    public VmL10n(IServiceProvider services, CreateParam param)
    {
        this.services = services;

        this.Name = param.Name;
        this.Title = this.Name;
        this.LoadFileCommand = new RelayCommand(this.OnLoadFile);

        this.TextFileName = CutFileIo.GetTextFileName(this.Name);
        var cutList = CutFileIo.LoadCutData(this.Name);
        if (cutList.Count == 0)
        {
            Log.Warn($"{this.Name} 파일에 컷이 없습니다.");
            return;
        }

        var uidGenerator = new CutUidGenerator(cutList);
        foreach (var cut in cutList.Where(e => e.Choices.Any()))
        {
            var choiceUidGenerator = new ChoiceUidGenerator(cut.Uid, cut.Choices);
        }

        this.originCuts = cutList.ToDictionary(e => e.Uid);

        int normalCut = 0;
        int branchCut = 0;
        foreach (var cut in cutList)
        {
            L10nMapping mapping;
            if (cut.Choices.Count == 0)
            {
                mapping = new L10nMapping(cut);
                this.mappings.Add(mapping.UidStr, mapping);
                ++normalCut;
            }
            else
            {
                foreach (var choice in cut.Choices)
                {
                    mapping = new L10nMapping(cut, choice);
                    this.mappings.Add(mapping.UidStr, mapping);
                    ++branchCut;
                }
            }
        }

        this.WriteLog($"전체 데이터 {this.originCuts.Count}개. 기본형 {normalCut}개, 선택지 {branchCut}개.");
    }

    public string Name { get; }
    public string TextFileName { get; }
    public ICommand LoadFileCommand { get; }
    public IEnumerable<L10nMapping> Mappings => this.mappings.Values;
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

    public int StatCountNormal => this.mappingStat[(int)L10nMappingType.Normal];
    public int StatCountMissingOrigin => this.mappingStat[(int)L10nMappingType.MissingOrigin];
    public int StatCountMissingImported => this.mappingStat[(int)L10nMappingType.MissingImported];
    public int StatCountTextChanged => this.mappingStat[(int)L10nMappingType.TextChanged];

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.ImportFilePath):
                this.OnPropertyChanged(nameof(this.ImportFileName));
                break;
        }
    }

    private void OnLoadFile()
    {
        var picker = this.services.GetRequiredService<IFilePicker>();
        this.ImportFilePath = picker.PickFile(Environment.CurrentDirectory, "엑셀 파일 (*.xlsx)|*.xlsx");
        if (this.importFilePath is null)
        {
            return;
        }

        this.importedCuts.Clear();
        this.importedMappings.Clear();
        this.importedHeaders.Clear();
        Array.Clear(this.mappingStat);

        foreach (var mapping in this.mappings.Values)
        {
            mapping.Imported = null;
        }

        var reader = this.services.GetRequiredService<IExcelFileReader>();
        if (reader.Read(this.importFilePath, this.importedHeaders, this.importedCuts) == false)
        {
            Log.Error($"엑셀 파일 읽기에 실패했습니다. fileName:{this.importFilePath}");
            return;
        }

        this.HasEnglish = this.importedHeaders.Contains("English");
        this.HasJapanese = this.importedHeaders.Contains("Japanese");
        this.HasChineseSimplified = this.importedHeaders.Contains("ChineseSimplified");

        // -------------------- mapping data --------------------
        foreach (var imported in this.importedCuts)
        {
            if (this.mappings.TryGetValue(imported.Uid, out var mapping) == false)
            {
                this.WriteLog($"컷을 찾을 수 없습니다. uid:{imported.Uid}");
                this.importedMappings.Add(imported);
                ++this.mappingStat[(int)L10nMappingType.MissingOrigin];
                continue;
            }

            mapping.Imported = imported;
            ++this.mappingStat[(int)mapping.MappingType];

            if (mapping.MappingType == L10nMappingType.TextChanged)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[Uid:{mapping.UidStr}] 한글 텍스트가 일치하지 않습니다.");
                sb.AppendLine($"  원본: {mapping.L10NText.Korean}");
                sb.Append($"  번역본: {imported.Korean}");
                this.WriteLog(sb.ToString());
            }
        }

        this.OnPropertyChanged(nameof(this.StatCountNormal));
        this.OnPropertyChanged(nameof(this.StatCountMissingOrigin));
        this.OnPropertyChanged(nameof(this.StatCountMissingImported));
        this.OnPropertyChanged(nameof(this.StatCountTextChanged));
    }

    private void WriteLog(string message)
    {
        var formatted = $"[{DateTime.Now:HH:mm:ss}] {message}";
        this.logMessages.Add(formatted);
    }

    public sealed record CreateParam(string Name);

    public sealed class Factory(IServiceProvider services)
    {
        public VmPageBase Create(CreateParam param)
        {
            return new VmL10n(services, param);
        }
    }
}