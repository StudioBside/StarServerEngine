﻿namespace CutEditor.ViewModel;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.ViewModel.Enums;

public sealed class VmCut : ObservableObject
{
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

        this.showUnitSection = true;
    }

    public Cut Cut { get; }
    public IRelayCommand PickUnitCommand { get; }
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
}
