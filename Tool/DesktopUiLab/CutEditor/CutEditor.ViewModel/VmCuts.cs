namespace CutEditor.ViewModel;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Core.Perforce;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using CutEditor.ViewModel.Detail;
using CutEditor.ViewModel.UndoCommands;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Du.Core.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Shared.Templet.Strings;
using Shared.Templet.TempletTypes;
using static CutEditor.Model.Messages;
using static CutEditor.ViewModel.Enums;

public sealed class VmCuts : VmPageBase,
    IClipboardHandler,
    ILoadEventReceiver
{
    private readonly CreateParam param;
    private readonly ObservableCollection<VmCut> cuts = [];
    private readonly ObservableCollection<VmCut> selectedCuts = [];
    private readonly string binFilePath;
    private readonly CutUidGenerator uidGenerator;
    private readonly IServiceProvider services;
    private readonly UndoController undoController;
    private readonly string packetExeFile;
    private readonly IServiceScope serviceScope;
    private bool isDirty;

    public VmCuts(IConfiguration config, IServiceProvider services, CreateParam param)
    {
        this.param = param;
        this.FindFlyout = new VmFindFlyout(this, this.cuts);
        this.CutPaster = new VmCutPaster(this);
        this.serviceScope = services.CreateScope();
        this.undoController = this.serviceScope.ServiceProvider.GetRequiredService<UndoController>();
        this.services = services;
        this.SaveCommand = new AsyncRelayCommand(this.OnSave, () => this.IsDirty);
        this.DeleteCommand = new RelayCommand(this.OnDelete, () => this.selectedCuts.Count > 0);
        this.NewCutCommand = new RelayCommand<CutDataType>(this.OnNewCut);
        this.DeletePickCommand = new RelayCommand<VmCut>(this.OnDeletePick);
        this.GoToReadPageCommand = new RelayCommand(this.OnGoToReadPage);
        this.ScrollByUidCommand = new AsyncRelayCommand(this.OnScrollByUid);
        this.BulkEditTextCommand = new AsyncRelayCommand(this.OnBulkEditText, () => this.selectedCuts.Count > 1);
        this.BulkEditCharacterCommand = new AsyncRelayCommand(this.OnBulkEditCharacter, () => this.selectedCuts.Count > 1);
        this.BulkEditUnitNameCommand = new AsyncRelayCommand(this.OnBulkEditUnitName, () => this.selectedCuts.Count > 1);
        WeakReferenceMessenger.Default.Register<UpdatePreviewMessage>(this, this.OnUpdatePreview);

        this.binFilePath = config["CutBinFilePath"] ?? throw new Exception("CutBinFilePath is not set in the configuration file.");
        this.packetExeFile = config["TextFilePacker"] ?? throw new Exception("TextFilePacker is not set in the configuration file.");

        this.Name = param.FileName;
        this.Title = this.Name;
        this.TextFileName = CutFileIo.GetTextFileName(this.Name);
        var cutList = CutFileIo.LoadCutData(this.Name);

        foreach (var cut in cutList)
        {
            this.cuts.Add(new VmCut(cut, this));
        }

        this.uidGenerator = new CutUidGenerator(this.cuts.Select(e => e.Cut));

        this.selectedCuts.CollectionChanged += this.SelectedCuts_CollectionChanged;

        this.cuts.CollectionChanged += (s, e) =>
        {
            this.IsDirty = true;
            this.UpdatePreview(startIndex: 0);
        };

        this.UpdatePreview(startIndex: 0);

        Log.Info($"{this.Name} 파일 로딩 완료. 총 컷의 개수:{this.cuts.Count}");
    }
  
    public string Name { get; }
    public IList<VmCut> Cuts => this.cuts;
    public IList<VmCut> SelectedCuts => this.selectedCuts;
    public UndoController UndoController => this.undoController;
    public VmFindFlyout FindFlyout { get; }
    public VmCutPaster CutPaster { get; }
    public string TextFileName { get; }
    public ICommand UndoCommand => this.undoController.UndoCommand;
    public ICommand RedoCommand => this.undoController.RedoCommand;
    public IRelayCommand SaveCommand { get; }
    public IRelayCommand DeleteCommand { get; } // 현재 (멀티)선택한 대상을 모두 삭제
    public ICommand NewCutCommand { get; }
    public ICommand DeletePickCommand { get; } // 인자로 넘어오는 1개의 cut을 삭제
    public ICommand GoToReadPageCommand { get; } // note: 제대로 하려면 수정사항을 저장하고 넘어가야 함.
    public ICommand ScrollByUidCommand { get; }
    public IRelayCommand BulkEditTextCommand { get; }
    public IRelayCommand BulkEditCharacterCommand { get; }
    public IRelayCommand BulkEditUnitNameCommand { get; }

    public bool IsDirty
    {
        get => this.isDirty;
        set => this.SetProperty(ref this.isDirty, value);
    }

    internal CutUidGenerator UidGenerator => this.uidGenerator;
    internal IServiceProvider Services => this.services;
    private string DebugName => $"[{this.Name}]";
    async Task<bool> IClipboardHandler.HandlePastedTextAsync(string text)
    {
        text = text.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        var sb = new StringBuilder();
        var tokens = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        sb.AppendLine($"다음의 텍스트를 이용해 {tokens.Length}개의 cut 데이터를 생성합니다.");
        sb.AppendLine();
        int previewCharacterCount = 60;
        if (text.Length > previewCharacterCount)
        {
            sb.AppendLine($"{text[..previewCharacterCount]} ... (and {text.Length - previewCharacterCount} more)");
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

        int positionIndex = this.cuts.Count - 1;
        if (this.selectedCuts.Count > 0)
        {
            positionIndex = this.cuts.IndexOf(this.selectedCuts[^1]);
        }

        var targets = new List<VmCut>();
        foreach (var token in tokens)
        {
            // 유닛이름 : 텍스트 형식인 경우, 이름을 파싱해 유효한 값인지 확인
            Unit? unit = null;
            string talkText = token;
            var idx = token.IndexOf(':');
            if (idx > 0)
            {
                var unitName = token[..idx].Trim();
                unit = Unit.Values.FirstOrDefault(e => e.Name == unitName);
                if (unit is not null)
                {
                    talkText = token[(idx + 1)..].Trim();
                }
            }

            var cut = new Cut(this.uidGenerator.Generate());
            cut.Unit = unit;

            // < ~ > 로 둘러싸인 경우 선택지 포맷으로 인식
            if (unit is null && talkText.StartsWith('<') && talkText.EndsWith('>'))
            {
                var newChoice = new ChoiceOption();
                newChoice.Text.Korean = talkText[1..^1];
                cut.Choices.Add(newChoice);
            }
            else
            {
                // 아닐 땐 일반 unitTalk.
                cut.UnitTalk.Korean = talkText;
                cut.TalkTime = Cut.TalkTimeDefault;
            }

            targets.Add(new VmCut(cut, this));
        }

        var command = new PasteCut(this, targets, positionIndex, PasteCut.PasteDirection.Downside)
        {
            ReserveOnUndo = false,
        };

        if (command is null)
        {
            return false;
        }

        command.Redo();
        this.undoController.Add(command);

        return true;
    }

    public override void OnNavigating(object sender, Uri uri)
    {
        // 다른 페이지로의 네이게이션이 시작될 때 (= 지금 페이지가 닫힐 때)
        Log.Debug($"{this.DebugName} OnNavigating: {uri}");

        this.serviceScope.Dispose();
    }

    public void UpdatePreview(int startIndex)
    {
        Log.Debug($"{this.DebugName} preview 업데이트 시작. startIndex:{startIndex}");

        Cut? prevCut = startIndex > 0
            ? this.cuts[startIndex - 1].Cut
            : null;

        for (int i = startIndex; i < this.cuts.Count; i++)
        {
            this.cuts[i].Cut.Preview.Calculate(prevCut);
            prevCut = this.cuts[i].Cut;
        }
    }

    public void OnLoaded()
    {
        if (this.param.CutUid == 0)
        {
            return;
        }

        var targetUid = this.param.CutUid;
        var targetCut = this.cuts.FirstOrDefault(e => e.Cut.Uid == targetUid);
        if (targetCut is null)
        {
            Log.Warn($"{this.DebugName} 네비게이션 대상 컷을 찾을 수 없습니다. cutUid:{targetUid}");
            return;
        }

        this.selectedCuts.Clear();
        this.selectedCuts.Add(targetCut);

        var index = this.cuts.IndexOf(targetCut);
        if (index < 0)
        {
            Log.Warn($"{this.DebugName} 네비게이션 대상 컷의 인덱스를 찾을 수 없습니다. cutUid:{targetUid}");
            return;
        }

        var controller = this.services.GetRequiredService<ICutsListController>();
        controller.ScrollIntoView(index);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SelectedCuts):
                this.DeleteCommand.NotifyCanExecuteChanged();
                this.BulkEditTextCommand.NotifyCanExecuteChanged();
                this.BulkEditCharacterCommand.NotifyCanExecuteChanged();
                this.BulkEditUnitNameCommand.NotifyCanExecuteChanged();
                break;

            case nameof(this.IsDirty):
                this.SaveCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void SelectedCuts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.DeleteCommand.NotifyCanExecuteChanged();
        this.BulkEditTextCommand.NotifyCanExecuteChanged();
        this.BulkEditCharacterCommand.NotifyCanExecuteChanged();
        this.BulkEditUnitNameCommand.NotifyCanExecuteChanged();
    }

    private async Task OnSave()
    {
        ////var waitingNotifier = this.services.GetRequiredService<IUserWaitingNotifier>();
        ////using var waiting = await waitingNotifier.StartWait($"{this.DebugName} 저장 중...");
        ////await Task.Delay(3000);
        await Task.Delay(0);

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

        var rows = this.cuts.Select(e => e.Cut.ToOutputJsonType())
            .Select(e => JsonConvert.SerializeObject(e, setting))
            .ToArray();

        var model = new
        {
            OutputFile = this.Name,
            Rows = rows,
        };

        template.Add("model", model);

        var textFilePath = this.TextFileName;
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
        var binFilePath = this.GetBinFileName();
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

    private void OnNewCut(CutDataType cutDataType)
    {
        var command = NewCut.Create(this, cutDataType);
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

    private string GetBinFileName() => Path.Combine(this.binFilePath, $"CLIENT_{this.Name}.bytes");

    private void OnUpdatePreview(object recipient, UpdatePreviewMessage message)
    {
        var vmCut = this.cuts.FirstOrDefault(e => e.Cut == message.Value);
        if (vmCut is null)
        {
            Log.Warn($"{this.DebugName} preview 대상 컷을 찾을 수 없습니다. cutUid:{message.Value.Uid}");
            return;
        }

        var startIndex = this.cuts.IndexOf(vmCut);
        this.UpdatePreview(startIndex);
    }

    private void OnGoToReadPage()
    {
        this.services.GetRequiredService<IPageRouter>()
            .Route(new VmCutsSummary.CreateParam(this.Name));
    }

    private async Task OnScrollByUid()
    {
        var prompt = this.services.GetRequiredService<IUserInputProvider<string>>();
        var uidStr = await prompt.PromptAsync("Uid로 이동", "이동할 컷의 Uid를 입력하세요.");
        if (int.TryParse(uidStr, out var uid) == false)
        {
            return;
        }

        var targetCut = this.cuts.FirstOrDefault(e => e.Cut.Uid == uid);
        if (targetCut is null)
        {
            Log.Warn($"{this.DebugName} 이동 대상 컷을 찾을 수 없습니다. cutUid:{uid}");
            return;
        }

        this.selectedCuts.Clear();
        this.selectedCuts.Add(targetCut);

        var index = this.cuts.IndexOf(targetCut);
        if (index < 0)
        {
            Log.Warn($"{this.DebugName} 이동 대상 컷의 인덱스를 찾을 수 없습니다. cutUid:{uid}");
        }

        var controller = this.services.GetRequiredService<ICutsListController>();
        controller.ScrollIntoView(index);
    }

    private async Task OnBulkEditUnitName()
    {
        var editor = this.services.GetRequiredKeyedService<IModelEditor<IList<StringElement>>>("unitName");
        var result = await editor.EditAsync([]);
        if (result.IsCanceled)
        {
            return;
        }

        foreach (var vmCut in this.selectedCuts)
        {
            vmCut.Cut.UnitNames.Clear();
            if (result.Data is not null)
            {
                foreach (var data in result.Data!)
                {
                    vmCut.Cut.UnitNames.Add(data);
                }
            }
        }
    }

    private async Task OnBulkEditCharacter()
    {
        var unitpicker = this.services.GetRequiredService<ITempletPicker<Unit>>();
        var result = await unitpicker.Pick();
        if (result.IsCanceled)
        {
            return;
        }

        foreach (var vmCut in this.selectedCuts)
        {
            vmCut.Cut.Unit = result.Data;
        }
    }

    private async Task OnBulkEditText()
    {
        var prompt = this.services.GetRequiredService<IUserInputProvider<string>>();
        var text = await prompt.PromptAsync("일괄 텍스트 변경", "변경할 텍스트를 입력하세요.");
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        foreach (var vmCut in this.selectedCuts)
        {
            vmCut.Cut.UnitTalk.Korean = text;
        }
    }

    public sealed record CreateParam(string FileName, long CutUid);

    public sealed class Factory(IServiceProvider services)
    {
        public VmPageBase Create(CreateParam param)
        {
            var config = services.GetRequiredService<IConfiguration>();
            return new VmCuts(config, services, param);
        }
    }
}
