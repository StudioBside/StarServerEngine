namespace CutEditor.ViewModel;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmCut : ObservableObject
{
    private readonly IServiceProvider services;

    public VmCut(Cut cut, IServiceProvider services)
    {
        this.Cut = cut;
        this.services = services;
        this.PickUnitCommand = new AsyncRelayCommand(this.OnPickUnit);
    }

    public Cut Cut { get; }
    public IRelayCommand PickUnitCommand { get; }

    //// --------------------------------------------------------------------------------------------

    private async Task OnPickUnit()
    {
        var unitpicker = this.services.GetRequiredService<IUnitPicker>();
        var unit = await unitpicker.PickUnit();
    }
}
