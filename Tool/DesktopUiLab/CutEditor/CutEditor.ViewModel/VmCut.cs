namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using CutEditor.ViewModel.Detail;
using CutEditor.ViewModel.UndoCommands;
using Du.Core.Interfaces;
using Du.Core.Util;
using Microsoft.Extensions.DependencyInjection;
using NKM;
using Shared.Templet.Strings;
using Shared.Templet.TempletTypes;
using static CutEditor.Model.Enums;
using static CutEditor.ViewModel.Enums;
using static Shared.Templet.Enums;

public sealed class VmCut : ObservableObject
{
    private readonly VmCuts owner;
    private readonly ChoiceUidGenerator choiceUidGenerator;
    private bool showUnitSection;
    private bool showScreenSection;
    private bool showCameraSection;
    private CutDataType dataType;
    private bool screenCrashFlyoutOpen;
    private bool slateFlyoutOpen;
    private bool minorityFlyoutOpen;

    public VmCut(Cut cut, VmCuts owner)
    {
        this.owner = owner;
        this.Cut = cut;
        cut.PropertyChanged += this.Cut_PropertyChanged;
        this.dataType = cut.Choices.Count > 0
            ? CutDataType.Branch
            : CutDataType.Normal;

        this.PickUnitCommand = new AsyncRelayCommand(this.OnPickUnit);
        this.PickArcpointCommand = new AsyncRelayCommand(this.OnPickArcpoint);
        this.PickBgmACommand = new AsyncRelayCommand(this.OnPickBgmA);
        this.PickBgmBCommand = new AsyncRelayCommand(this.OnPickBgmB);
        this.PickSfxACommand = new AsyncRelayCommand(this.OnPickSfxA);
        this.PickSfxBCommand = new AsyncRelayCommand(this.OnPickSfxB);
        this.PickVoiceCommand = new AsyncRelayCommand(this.OnPickVoice);
        this.PickAmbientSoundCommand = new AsyncRelayCommand(this.OnPickAmbientSound);
        this.PickBgFileNameCommand = new AsyncRelayCommand(this.OnPickBgFileName);
        this.PickCameraEaseCommand = new AsyncRelayCommand(this.OnPickCameraEase);
        this.PickCameraOffsetCommand = new AsyncRelayCommand(this.OnPickCameraOffset);
        this.AddChoiceOptionCommand = new RelayCommand(this.OnAddChoiceOption, () => this.Cut.Choices.Count < 5);
        this.EditChoiceOptionCommand = new AsyncRelayCommand<ChoiceOption>(this.OnEditchoiceOption);
        this.DeleteChoiceOptionCommand = new RelayCommand<ChoiceOption>(this.OnDeleteChoiceOption, _ => this.Cut.Choices.Count > 1);
        this.SetAnchorCommand = new RelayCommand<DestAnchorType>(e => this.Cut.JumpAnchor = e);
        this.SetEmotionEffectCommand = new RelayCommand<EmotionEffect>(e => this.Cut.EmotionEffect = e);
        this.SetUnitMotionCommand = new RelayCommand<string>(this.OnSetUnitMotion);
        this.SetUnitNameStringCommand = new AsyncRelayCommand(this.OnSetUnitNameString);
        this.SetTransitionEffectCommand = new RelayCommand<TransitionEffect>(e => this.Cut.TransitionEffect = e);
        this.SetTransitionControlCommand = new RelayCommand<TransitionControl>(this.OnSetTransitionControl);
        this.SetAutoHighlightCommand = new RelayCommand<CutsceneAutoHighlight>(e => this.Cut.AutoHighlight = e);
        this.SetFilterTypeCommand = new RelayCommand<CutsceneFilterType>(e => this.Cut.FilterType = e);
        this.SetCutsceneClearCommand = new RelayCommand<CutsceneClearType>(e => this.Cut.CutsceneClear = e);
        this.OpenScreenCrashFlyoutCommand = new RelayCommand(() => this.ScreenCrashFlyoutOpen = true);
        this.ClearScreenFlashCrashCommand = new RelayCommand(this.OnClearScreenFlashCrash);
        this.OpenSlateFlyoutCommand = new RelayCommand(() => this.SlateFlyoutOpen = true);
        this.OpenMinorityFlyoutCommand = new RelayCommand(() => this.MinorityFlyoutOpen = true);
        this.ClearSlateFlyoutCommand = new RelayCommand(this.OnClearSlateControl);
        this.SetStartFxLoopCommand = new RelayCommand<CutsceneSoundLoopControl>(e => this.Cut.StartFxLoopControl = e);
        this.SetEndFxLoopCommand = new RelayCommand<CutsceneSoundLoopControl>(e => this.Cut.EndFxLoopControl = e);
        this.EditBgFadeCommand = new AsyncRelayCommand(this.OnEditBgFade);
        this.SetTalkPositionControlCommand = new RelayCommand<TalkPositionControlType>(e => this.Cut.TalkPositionControl = e);
        this.ClearUnitPosCommand = new RelayCommand(this.OnClearUnitPos);
        this.MakeChangePosHistoryCommand = new RelayCommand<string>(this.OnMakeChangePosHistory);

        this.showUnitSection = cut.HasUnitData();
        this.showScreenSection = cut.HasScreenBoxData();
        this.ShowCameraSection = cut.HasCameraBoxData();

        this.choiceUidGenerator = new(cut.Uid, cut.Choices);

        if (this.Cut.Choices is ObservableCollection<ChoiceOption> choices)
        {
            choices.CollectionChanged += (s, e) =>
            {
                this.AddChoiceOptionCommand.NotifyCanExecuteChanged();
                this.DeleteChoiceOptionCommand.NotifyCanExecuteChanged();
            };
        }
    }

    public Cut Cut { get; }
    public IRelayCommand PickUnitCommand { get; }
    public ICommand PickArcpointCommand { get; }
    public ICommand PickBgmACommand { get; }
    public ICommand PickBgmBCommand { get; }
    public ICommand PickSfxACommand { get; }
    public ICommand PickSfxBCommand { get; }
    public ICommand PickVoiceCommand { get; }
    public ICommand PickAmbientSoundCommand { get; }
    public ICommand PickBgFileNameCommand { get; }
    public ICommand PickCameraEaseCommand { get; }
    public ICommand PickCameraOffsetCommand { get; }
    public IRelayCommand AddChoiceOptionCommand { get; }
    public ICommand EditChoiceOptionCommand { get; }
    public IRelayCommand DeleteChoiceOptionCommand { get; }
    public ICommand SetAnchorCommand { get; }
    public ICommand SetEmotionEffectCommand { get; }
    public ICommand SetUnitMotionCommand { get; }
    public ICommand SetUnitNameStringCommand { get; }
    public ICommand SetTransitionEffectCommand { get; }
    public ICommand SetTransitionControlCommand { get; }
    public ICommand SetAutoHighlightCommand { get; }
    public ICommand SetFilterTypeCommand { get; }
    public ICommand SetCutsceneClearCommand { get; }
    public ICommand OpenScreenCrashFlyoutCommand { get; }
    public ICommand ClearScreenFlashCrashCommand { get; }
    public ICommand OpenSlateFlyoutCommand { get; }
    public ICommand OpenMinorityFlyoutCommand { get; }
    public ICommand ClearSlateFlyoutCommand { get; }
    public ICommand SetStartFxLoopCommand { get; }
    public ICommand SetEndFxLoopCommand { get; }
    public ICommand EditBgFadeCommand { get; }
    public ICommand SetTalkPositionControlCommand { get; }
    public ICommand ClearUnitPosCommand { get; }
    public ICommand MakeChangePosHistoryCommand { get; }
    public bool ShowUnitSection
    {
        get => this.showUnitSection;
        set => this.SetProperty(ref this.showUnitSection, value);
    }

    public bool ShowScreenSection
    {
        get => this.showScreenSection;
        set => this.SetProperty(ref this.showScreenSection, value);
    }

    public bool ShowCameraSection
    {
        get => this.showCameraSection;
        set => this.SetProperty(ref this.showCameraSection, value);
    }

    public CutDataType DataType
    {
        get => this.dataType;
        set => this.SetProperty(ref this.dataType, value);
    }

    public bool ScreenCrashFlyoutOpen
    {
        get => this.screenCrashFlyoutOpen;
        set => this.SetProperty(ref this.screenCrashFlyoutOpen, value);
    }

    public bool SlateFlyoutOpen
    {
        get => this.slateFlyoutOpen;
        set => this.SetProperty(ref this.slateFlyoutOpen, value);
    }

    public bool MinorityFlyoutOpen
    {
        get => this.minorityFlyoutOpen;
        set => this.SetProperty(ref this.minorityFlyoutOpen, value);
    }

    private IServiceProvider Services => this.owner.Services;
    private UndoController UndoController => this.owner.UndoController;

    public override string ToString() => $"VmCut. Uid:{this.Cut.Uid}";

    //// --------------------------------------------------------------------------------------------

    private void Cut_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Model.Cut.EmotionEffect):
                break;

            case nameof(Model.Cut.CameraOffset):
                this.ShowCameraSection = this.Cut.HasCameraBoxData();
                break;
        }

        this.owner.IsDirty = true;
    }

    private async Task OnPickUnit()
    {
        var unitpicker = this.Services.GetRequiredService<ITempletPicker<Unit>>();
        var result = await unitpicker.Pick();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.Unit = result.Data;
    }

    private async Task OnPickArcpoint()
    {
        var picker = this.Services.GetRequiredService<ITempletPicker<LobbyItem>>();
        var result = await picker.Pick();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.Arcpoint = result.Data;
    }

    private async Task OnPickBgmA()
    {
        var bgmpicker = this.Services.GetRequiredKeyedService<IAssetPicker>("bgm");
        var result = await bgmpicker.PickAsset();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.StartBgmFileName = result.Data;
    }

    private async Task OnPickBgmB()
    {
        var bgmpicker = this.Services.GetRequiredKeyedService<IAssetPicker>("bgm");
        var result = await bgmpicker.PickAsset();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.EndBgmFileName = result.Data;
    }

    private async Task OnPickSfxA()
    {
        var sfxPicker = this.Services.GetRequiredKeyedService<IAssetPicker>("sfx");
        var result = await sfxPicker.PickAsset();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.StartFxSoundName = result.Data;
    }

    private async Task OnPickSfxB()
    {
        var sfxPicker = this.Services.GetRequiredKeyedService<IAssetPicker>("sfx");
        var result = await sfxPicker.PickAsset();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.EndFxSoundName = result.Data;
    }

    private async Task OnPickVoice()
    {
        var voicePicker = this.Services.GetRequiredKeyedService<IAssetPicker>("voice");
        var result = await voicePicker.PickAsset();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.TalkVoice = result.Data;
    }

    private async Task OnPickAmbientSound()
    {
        var sfxPicker = this.Services.GetRequiredKeyedService<IAssetPicker>("sfx");
        var result = await sfxPicker.PickAsset();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.AmbientSound = result.Data;
    }

    private async Task OnPickBgFileName()
    {
        var bgFilePicker = this.Services.GetRequiredKeyedService<IAssetPicker>("bgFile");
        var result = await bgFilePicker.PickAsset(this.Cut.BgFileName);
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.BgFileName = result.Data;
    }

    private async Task OnPickCameraEase()
    {
        var picker = this.Services.GetRequiredService<IEnumPicker<Ease>>();
        var result = await picker.Pick(this.Cut.CameraEase);
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.CameraEase = result.Data;
    }

    private async Task OnPickCameraOffset()
    {
        var picker = this.Services.GetRequiredService<IEnumPicker<CameraOffset>>();
        var result = await picker.Pick(this.Cut.CameraOffset);
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.CameraOffset = result.Data;
    }

    private void OnAddChoiceOption()
    {
        long newUid = this.choiceUidGenerator.Generate();
        var newChoice = new ChoiceOption();
        newChoice.InitializeUid(this.Cut.Uid, newUid);
        newChoice.Text.Korean = newChoice.UidString;

        this.Cut.Choices.Add(newChoice);
    }

    private void OnDeleteChoiceOption(ChoiceOption? target)
    {
        if (target is null)
        {
            throw new Exception($"remove target is null");
        }

        this.Cut.Choices.Remove(target);
    }

    private void OnSetUnitMotion(string? unitMotion)
    {
        if (unitMotion == AssetList.UnitMotionEmpty)
        {
            this.Cut.UnitMotion = null;
            return;
        }

        this.Cut.UnitMotion = unitMotion;
    }

    private async Task OnSetUnitNameString()
    {
        var editor = this.Services.GetRequiredKeyedService<IModelEditor<IList<StringElement>>>("unitName");
        var result = await editor.EditAsync(this.Cut.UnitNames);
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.UnitNames.Clear();
        if (result.Data is not null)
        {
            foreach (var data in result.Data!)
            {
                this.Cut.UnitNames.Add(data);
            }
        }
    }

    private void OnSetTransitionControl(TransitionControl transitionControl)
    {
        if (transitionControl == TransitionControl.NONE)
        {
            this.Cut.TransitionEffect = null;
            this.Cut.TransitionControl = null;
            return;
        }

        this.Cut.TransitionControl = transitionControl;
    }

    private async Task OnEditchoiceOption(ChoiceOption? target)
    {
        if (target is null)
        {
            throw new Exception($"remove target is null");
        }

        IUserInputProvider<string> userInputProvider = this.Services.GetRequiredService<IUserInputProvider<string>>();
        string defaultValue = target.Text.Korean;
        var result = await userInputProvider.PromptAsync("선택지 텍스트를 입력하세요", "선택지 텍스트", defaultValue);
        if (string.IsNullOrWhiteSpace(result))
        {
            return;
        }

        target.Text.Korean = result;
    }

    private void OnClearScreenFlashCrash()
    {
        this.Cut.BgFlashBang = 0f;
        this.Cut.BgCrash = 0f;
        this.Cut.BgCrashTime = 0f;
    }

    private void OnClearSlateControl()
    {
        this.Cut.SlateControlType = SlateControlType.NONE;
        this.Cut.SlateSectionNo = 0;
    }

    private async Task OnEditBgFade()
    {
        var editor = this.Services.GetRequiredService<IModelEditor<BgFadeInOut>>();
        var result = await editor.EditAsync(this.Cut.BgFadeInOut);

        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.BgFadeInOut = result.Data;
    }

    private void OnClearUnitPos()
    {
        var command = SetUnitPos.Create(this, CutsceneUnitPos.NONE);
        if (command is null)
        {
            return;
        }

        command.Redo();
        this.UndoController.Add(command);
    }

    private void OnMakeChangePosHistory(string? posStr)
    {
        if (Enum.TryParse<CutsceneUnitPos>(posStr, out var unitPos) == false)
        {
            return;
        }

        var command = SetUnitPos.Create(this, unitPos);
        if (command is null)
        {
            return;
        }

        command.Redo();
        this.UndoController.Add(command);
    }
}