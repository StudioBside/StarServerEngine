namespace CutEditor.Dialogs;

using System.Windows.Controls;
using CutEditor.Dialogs.CameraOffsetPicker;
using NKM;
using Wpf.Ui.Controls;

public partial class CameraOffsetPickerDialog : ContentDialog
{
    private readonly CameraOffsetPickerVm vm;
    public CameraOffsetPickerDialog(CameraOffset current, ContentPresenter? dialogHost) : base(dialogHost)
    {
        this.vm = new CameraOffsetPickerVm(current);
        this.DataContext = this.vm;

        this.InitializeComponent();
    }

    public CameraOffset Result => this.vm.Result;
}
