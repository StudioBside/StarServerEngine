﻿namespace CutEditor.ViewModel;

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Core.Perforce;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.ViewModel.Detail;
using CutEditor.ViewModel.UndoCommands;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Du.Core.Models;
using Du.Core.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

public sealed class VmCuts : VmPageBase,
    IDragDropHandler,
    IClipboardHandler
{
    private readonly ObservableCollection<VmCut> cuts = new();
    private readonly ObservableCollection<VmCut> selectedCuts = new();
    private readonly string textFilePath;
    private readonly string binFilePath;
    private readonly string name;
    private readonly TempUidGenerator uidGenerator = new();
    private readonly IServiceProvider services;
    private readonly UndoController undoController = new();
    private readonly string packetExeFile;
    private bool showSummary;

    public VmCuts(IConfiguration config, IServiceProvider services)
    {
        LastInstance = this;

        this.services = services;
        this.BackCommand = new RelayCommand(this.OnBack);
        this.SaveCommand = new RelayCommand(this.OnSave);
        this.DeleteCommand = new RelayCommand(this.OnDelete);
        this.NewCutCommand = new RelayCommand(this.OnNewCut);
        this.DeletePickCommand = new RelayCommand<VmCut>(this.OnDeletePick);

        if (GlobalState.Instance.PopVmCuts(out var param) == false)
        {
            throw new Exception($"VmCuts.CreateParam is not set in the GlobalState.");
        }

        this.textFilePath = config["CutTextFilePath"] ?? throw new Exception("CutTextFilePath is not set in the configuration file.");
        this.binFilePath = config["CutBinFilePath"] ?? throw new Exception("CutBinFilePath is not set in the configuration file.");
        this.packetExeFile = config["TextFilePacker"] ?? throw new Exception("TextFilePacker is not set in the configuration file.");

        if (param.CutScene is null)
        {
            this.name = param.NewFileName ?? throw new Exception("invalid createParam. newFileName is empty.");
            this.Title = $"새로운 파일 생성 - {this.name}";
        }
        else
        {
            this.name = param.CutScene.FileName;
            this.Title = $"{param.CutScene.Title} - {this.name}";
        }

        var textFilePath = this.GetTextFilePath();
        if (File.Exists(textFilePath) == false)
        {
            Log.Debug($"cutscene file not found: {textFilePath}");
            return;
        }

        var json = JsonUtil.Load(textFilePath);
        json.GetArray("Data", this.cuts, (e, i) =>
        {
            var cut = new Cut(e, this.uidGenerator.Generate());
            return new VmCut(cut, this.services);
        });

        Log.Info($"{this.name} 파일 로딩 완료. 총 컷의 개수:{this.cuts.Count}");
    }

    public static VmCuts? LastInstance { get; private set; }
    public IList<VmCut> Cuts => this.cuts;
    public IList<VmCut> SelectedCuts => this.selectedCuts;
    public ICommand UndoCommand => this.undoController.UndoCommand;
    public ICommand RedoCommand => this.undoController.RedoCommand;
    public ICommand BackCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; } // 현재 (멀티)선택한 대상을 모두 삭제
    public ICommand NewCutCommand { get; }
    public ICommand DeletePickCommand { get; } // 인자로 넘어오는 1개의 cut을 삭제
    public bool ShowSummary
    {
        get => this.showSummary;
        set => this.SetProperty(ref this.showSummary, value);
    }

    internal TempUidGenerator UidGenerator => this.uidGenerator;
    internal IServiceProvider Services => this.services;
    private string DebugName => $"[{this.name}]";

    bool IDragDropHandler.HandleDrop(object listViewContext, IList selectedItems, object targetContext)
    {
        if (targetContext is not VmCut dropTarget)
        {
            Log.Error("targetContext is not VmCut.");
            return false;
        }

        var items = selectedItems.Cast<VmCut>().ToList();

        var dropIndex = this.cuts.IndexOf(dropTarget);
        var itemsIndex = items.Select(this.cuts.IndexOf).OrderByDescending(i => i).ToList();
        if (itemsIndex.Count == 0)
        {
            return false;
        }

        // drop 대상이 items에 속해있으면 에러
        if (itemsIndex.Contains(dropIndex))
        {
            //Log.Error("drop target is in the moving items.");
            return false;
        }

        var indices = itemsIndex.Count == 1
            ? itemsIndex[0].ToString()
            : string.Join(", ", itemsIndex.OrderBy(e => e));

        // itemsIndex가 연속적이지 않으면 에러
        if (itemsIndex.Count > 1)
        {
            var min = itemsIndex.Min();
            var max = itemsIndex.Max();
            if (max - min + 1 != itemsIndex.Count)
            {
                Log.Warn($"위치를 이동할 아이템은 연속적으로 선택해야 합니다. 현재 선택 인덱스:{indices}");
                return false;
            }
        }

        Log.Info($"위치 조정: {indices} -> {dropIndex}");

        var movingItems = new List<VmCut>();
        foreach (var i in itemsIndex)
        {
            movingItems.Add(this.cuts[i]);
            this.cuts.RemoveAt(i);
        }

        // 더 아래로 내리는 경우는 목적지 인덱스가 바뀔테니 재계산 필요
        if (dropIndex > itemsIndex.Max())
        {
            dropIndex -= itemsIndex.Count - 1;
        }

        foreach (var item in movingItems)
        {
            this.cuts.Insert(dropIndex, item);
        }

        return true;
    }

    async Task<bool> IClipboardHandler.HandlePastedTextAsync(string text)
    {
        text = text.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        var sb = new StringBuilder();
        var tokens = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        sb.AppendLine($"다음의 텍스트를 이용해 {tokens.Length}개의 cut 데이터를 생성합니다.");
        sb.AppendLine();
        if (text.Length > 10)
        {
            sb.AppendLine($"{text[..10]} ... (and {text.Length - 10} more)");
        }
        else
        {
            sb.AppendLine(text);
        }

        var boolProvider = this.services.GetRequiredService<IUserInputProvider<bool>>();
        if (await boolProvider.PromptAsync("새로운 Cut을 만듭니다", sb.ToString()) == false)
        {
            return false;
        }

        foreach (var token in tokens)
        {
            var cut = new Cut(this.uidGenerator.Generate());
            cut.UnitTalk.Korean = token;
            this.cuts.Add(new VmCut(cut, this.services));
        }

        return true;
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        //switch (e.PropertyName)
        //{
        //    case nameof(this.SearchKeyword):
        //        this.filteredList.Refresh(this.searchKeyword);
        //        break;

        //    case nameof(this.SelectedCutScene):
        //        this.StartEditCommand.NotifyCanExecuteChanged();
        //        break;
        //}
    }

    private void OnBack()
    {
        WeakReferenceMessenger.Default.Send(new NavigationMessage("GoBack"));
    }

    private void OnSave()
    {
        var textFilePath = this.GetTextFilePath();
        if (P4Commander.TryCreate(out var p4Commander) == false)
        {
            Log.Error($"{this.DebugName} P4Commander 객체 생성 실패");
            return;
        }

        if (p4Commander.Stream.Contains("/alpha"))
        {
            // 컷 데이터파일은 현재 p4 설저상 임포트(import+) 되어있어서, depot address에는 dev로 표기됩니다.
            p4Commander = p4Commander with { Stream = "//stream/dev" };
        }

        // -------------------------- save text file --------------------------
        var template = StringTemplateFactory.Instance.GetTemplet("CutsOutput", "writeFile");
        if (template is null)
        {
            Log.Error($"{this.DebugName} template not found: CutsOutput.writeFile");
            return;
        }

        var setting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters =
            [
                new StringEnumConverter(),
            ],
        };

        var rows = this.cuts.Select(e => e.Cut.ToOutputType())
            .Select(e => JsonConvert.SerializeObject(e, setting))
            .ToArray();

        var model = new
        {
            OutputFile = this.name,
            Rows = rows,
        };

        template.Add("model", model);

        if (File.Exists(textFilePath))
        {
            File.SetAttributes(textFilePath, FileAttributes.Normal);
        }

        using (var sw = new StreamWriter(textFilePath, append: false, Encoding.UTF8))
        {
            sw.WriteLine(template.Render());
        }

        if (OpenForEdit(p4Commander, textFilePath, "text 파일") == false)
        {
            return;
        }

        // -------------------------- save binary file --------------------------
        var binFilePath = this.GetBinFilePath();
        if (OutProcess.Run(this.packetExeFile, $"\"{textFilePath}\" \"{binFilePath}\"", out string result) == false)
        {
            Log.Error($"{this.DebugName} binary 파일 생성 실패.\result:{result}");
            return;
        }

        if (OpenForEdit(p4Commander, binFilePath, "bin 파일") == false)
        {
            return;
        }

        var jObject = JsonConvert.DeserializeObject<JObject>(result) ?? throw new Exception("result is not JObject");
        long textFileSize = jObject.GetInt64("TextFileSize");
        //long bsonFileSize = jObject.GetInt64("BsonFileSize");
        long binFileSize = jObject.GetInt64("BinFileSize");
        float downRate = binFileSize * 100f / textFileSize;

        var sb = new StringBuilder();
        sb.AppendLine($"{this.DebugName} 파일을 저장했습니다.");
        sb.AppendLine($"- 텍스트 파일: {textFileSize.ToByteFormat()}");
        sb.AppendLine($"- 바이트 파일: {binFileSize.ToByteFormat()} ({downRate:0.00}%)");
        Log.Info(sb.ToString());

        // -- local function
        static bool OpenForEdit(P4Commander p4Commander, string filePath, string name)
        {
            if (p4Commander.CheckIfOpened(filePath) != false)
            {
                return true;
            }

            if (p4Commander.CheckIfChanged(filePath, out bool changed) == false)
            {
                Log.Error($"{name} 변경 여부 확인 실패.\n전체경로:{filePath}");
                return false;
            }

            if (changed == false)
            {
                Log.Info($"{name} 변경사항이 확인되지 않았습니다.\n전체경로:{filePath}");
                return false;
            }

            if (p4Commander.OpenForEdit(filePath, out string p4Output) == false)
            {
                Log.Error($"{name} 오픈 실패.\n전체경로:{filePath}");
                return false;
            }

            return true;
        }
    }

    private void OnDelete()
    {
        var command = DeleteCuts.Create(this);
        if (command is null)
        {
            return;
        }

        command.Redo();
        this.undoController.Add(command);
    }

    private void OnNewCut()
    {
        var command = NewCut.Create(this);
        command.Redo();
        this.undoController.Add(command);
    }

    private void OnDeletePick(VmCut? cut)
    {
        if (cut is null)
        {
            Log.Error($"{this.DebugName} 삭제 대상을 확인할 수 없습니다.");
            return;
        }

        var command = DeleteCuts.Create(this, cut);
        if (command is null)
        {
            return;
        }

        command.Redo();
        this.undoController.Add(command);
    }

    private string GetTextFilePath() => Path.Combine(this.textFilePath, $"CLIENT_{this.name}.exported");
    private string GetBinFilePath() => Path.Combine(this.binFilePath, $"CLIENT_{this.name}.bytes");

    public sealed record CrateParam
    {
        public CutScene? CutScene { get; init; }
        public string? NewFileName { get; init; }
    }
}