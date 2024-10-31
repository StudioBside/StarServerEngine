namespace CutEditor.Dialogs;

using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

public partial class BgFilePickerDialog : ContentDialog
{
    private readonly BgFilePickerDialogVm viewModel;

    public BgFilePickerDialog(string title, string? defaultValue, ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        this.viewModel = new BgFilePickerDialogVm();
        this.DataContext = this.viewModel;

        this.Title = title;
        this.InitializeComponent();

        // initializeComponent 호출 후에 기본값을 설정해준다.
        this.viewModel.Selected = new BgFilePickerDialogVm.ElementType(defaultValue ?? string.Empty);
    }

    public string? SelectedFileName => this.viewModel.Selected?.FileNameOnly;

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.viewModel.Selected is null)
        {
            this.InfoBarWarning.Visibility = Visibility.Visible;
            return;
        }

        base.OnButtonClick(button);
    }
}
