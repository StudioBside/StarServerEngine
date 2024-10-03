namespace CutEditor.Dialogs;

using CutEditor.Model;
using Wpf.Ui.Controls;

public partial class UnitPickerDialog : ContentDialog
{
    private readonly VmUnitPicker viewModel;

    public UnitPickerDialog(VmUnitPicker vmUnitPicker)
    {
        this.viewModel = vmUnitPicker;
        this.DataContext = vmUnitPicker;
        this.InitializeComponent();
    }

    public Unit? SelectedUnit => this.viewModel.SelectedUnit;
}
