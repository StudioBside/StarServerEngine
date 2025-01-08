namespace CutEditor.Model.CutSearch.Conditions;

using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.TempletTypes;

public sealed class ConditionUnitMatch : ObservableObject,
    ISearchCondition
{
    private readonly IServiceProvider services;
    private Unit unit = Unit.Values.First();

    public ConditionUnitMatch(IServiceProvider services)
    {
        this.services = services;
        this.SelectUnitCommand = new AsyncRelayCommand(this.OnSelectUnit);
    }

    bool ISearchCondition.IsValid => this.Unit != null;

    public Unit Unit
    {
        get => this.unit;
        set => this.SetProperty(ref this.unit, value);
    }

    public ICommand SelectUnitCommand { get; }

    public bool IsTarget(Cut cut)
    {
        return cut.Unit == this.Unit;
    }

    //// --------------------------------------------------------------------------------------------

    private async Task OnSelectUnit()
    {
        var unitpicker = this.services.GetRequiredService<IGeneralPicker<Unit>>();
        var result = await unitpicker.Pick();
        if (result.IsCanceled)
        {
            return;
        }

        this.Unit = result.Data!;
    }
}
