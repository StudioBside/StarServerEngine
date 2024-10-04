namespace CutEditor.Dialogs;

using System;
using System.Windows;
using CutEditor.Model;
using Shared.Templet.Base;
using Shared.Templet.TempletTypes;
using Wpf.Ui.Controls;

public partial class UnitPickerDialog : ContentDialog
{
    public UnitPickerDialog()
    {
        this.DataContext = this;

        this.InitializeComponent();
    }

    public IEnumerable<Unit> Units => TempletContainer<Unit>.Values;
    public Unit? SelectedUnit { get; set; }

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.SelectedUnit is null)
        {
            this.InfoBarWarning.Visibility = Visibility.Visible;
            return;
        }

        base.OnButtonClick(button);
    }

    protected override void OnClosed(ContentDialogResult result)
    {
        if (result != ContentDialogResult.Primary)
        {
            // 선택 버튼을 누르지 않은 경우는 선택 사항을 취소시킨다.
            this.SelectedUnit = null;
        }

        base.OnClosed(result);
    }
}
