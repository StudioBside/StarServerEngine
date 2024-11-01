namespace CutEditor.Dialogs;

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CutEditor.Dialogs.BgFilePicker;
using Wpf.Ui.Controls;

public partial class BgFilePickerDialog : ContentDialog
{
    private readonly BgFilePickerDialogVm viewModel;

    public BgFilePickerDialog(string title, string? defaultValue, ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        this.viewModel = new BgFilePickerDialogVm();
        this.DataContext = this.viewModel;

        this.PreviewKeyDown += this.BgFilePickerDialog_PreviewKeyDown;

        this.Title = title;
        this.InitializeComponent();

        // initializeComponent 호출 후에 기본값을 설정해준다.
        this.viewModel.Selected = defaultValue;
    }

    public string? SelectedFileName => this.viewModel.Selected;

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && string.IsNullOrEmpty(this.viewModel.Selected))
        {
            this.InfoBarWarning.Visibility = Visibility.Visible;
            return;
        }

        base.OnButtonClick(button);
    }

    private void BgFilePickerDialog_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // SearchBox로 포커스를 옮기고 키 입력을 처리합니다.
        if (this.SearchBox.IsFocused == false)
        {
            this.SearchBox.Focus();

            // 키 입력을 SearchBox로 전달합니다.
            string? addText = null;
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                addText = e.Key.ToString();
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                addText = e.Key.ToString().Substring(1);
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                addText = e.Key.ToString().Substring(6);
            }
            else if (e.Key == Key.Space)
            {
                addText = " ";
            }
            else if (e.Key == Key.OemPlus)
            {
                addText = "+";
            }
            else if (e.Key == Key.OemMinus)
            {
                addText = "-";
            }

            if (addText != null)
            {
                var textBox = this.SearchBox;
                textBox.Text += addText;
                textBox.CaretIndex = textBox.Text.Length;
            }

            e.Handled = true;
        }
    }
}
