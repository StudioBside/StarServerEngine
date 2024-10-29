namespace CutEditor.ViewModel;

using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using CutEditor.ViewModel.Detail;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.Model.Enums;
using static CutEditor.ViewModel.Enums;

public sealed class VmCut : ObservableObject
{
    private readonly ChoiceUidGenerator choiceUidGenerator;
    private readonly IServiceProvider services;
    private bool showUnitSection;
    private bool showScreenSection;
    private bool showCameraSection;
    private bool showSoundASection;
    private bool showSoundBSection;
    private CutDataType dataType;

    public VmCut(Cut cut, IServiceProvider services)
    {
        this.Cut = cut;
        this.dataType = cut.Choices.Count > 0
            ? CutDataType.Branch
            : CutDataType.Normal;

        this.services = services;
        this.PickUnitCommand = new AsyncRelayCommand(this.OnPickUnit);
        this.AddChoiceOptionCommand = new RelayCommand(this.OnAddChoiceOption);
        this.DeleteChoiceOptionCommand = new RelayCommand<ChoiceOption>(this.OnDeleteChoiceOption);
        this.SetAnchorCommand = new RelayCommand<DestAnchorType>(this.OnSetAnchor);

        this.showUnitSection = true;
        if (string.IsNullOrEmpty(cut.StartBgmFileName) == false ||
            string.IsNullOrEmpty(cut.StartFxSoundName) == false)
        {
            this.showSoundASection = true;
        }

        if (string.IsNullOrEmpty(cut.EndBgmFileName) == false ||
            string.IsNullOrEmpty(cut.EndFxSoundName) == false)
        {
            this.showSoundBSection = true;
        }

        this.choiceUidGenerator = new(cut.Uid);
        this.choiceUidGenerator.Initialize(cut.Choices);
    }

    public Cut Cut { get; }
    public IRelayCommand PickUnitCommand { get; }
    public ICommand AddChoiceOptionCommand { get; }
    public ICommand DeleteChoiceOptionCommand { get; }
    public ICommand SetAnchorCommand { get; }
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

    public bool ShowSoundASection
    {
        get => this.showSoundASection;
        set => this.SetProperty(ref this.showSoundASection, value);
    }

    public bool ShowSoundBSection
    {
        get => this.showSoundBSection;
        set => this.SetProperty(ref this.showSoundBSection, value);
    }

    public string SummaryText => this.Cut.GetSummaryText();

    public CutDataType DataType
    {
        get => this.dataType;
        set => this.SetProperty(ref this.dataType, value);
    }

    //// --------------------------------------------------------------------------------------------

    private async Task OnPickUnit()
    {
        var unitpicker = this.services.GetRequiredService<IUnitPicker>();
        var result = await unitpicker.PickUnit();
        if (result.IsCanceled)
        {
            return;
        }

        this.Cut.Unit = result.Unit;
    }

    private void OnAddChoiceOption()
    {
        long newUid = this.choiceUidGenerator.GenerateNewUid();
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

    private void OnSetAnchor(DestAnchorType type)
    {
        this.Cut.JumpAnchor = type;
    }
}