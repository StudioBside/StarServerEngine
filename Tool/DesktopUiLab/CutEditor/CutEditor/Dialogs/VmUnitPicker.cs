namespace CutEditor.Dialogs;

using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model;
using Wpf.Ui.Controls;

public sealed class VmUnitPicker : ObservableObject
{
    private readonly UnitContainer unitContainer;
    private Unit? selectedUnit;
    private bool showWarning;

    public VmUnitPicker(UnitContainer unitContainer)
    {
        this.unitContainer = unitContainer;
        this.ClosingCommand = new RelayCommand<ContentDialogClosingEventArgs>(this.OnClosing);
    }

    public IEnumerable<Unit> Units => this.unitContainer.Units;
    public Unit? SelectedUnit
    {
        get => this.selectedUnit;
        set => this.SetProperty(ref this.selectedUnit, value);
    }

    public bool ShowWarning
    {
        get => this.showWarning;
        set => this.SetProperty(ref this.showWarning, value);
    }

    public ICommand ClosingCommand { get; }

    private void OnClosing(ContentDialogClosingEventArgs? args)
    {
        if (args is null)
        {
            return;
        }

        if (args.Result == ContentDialogResult.Primary && this.SelectedUnit is null)
        {
            this.ShowWarning = true;
            args.Cancel = true;
            return;
        }
    }
}
