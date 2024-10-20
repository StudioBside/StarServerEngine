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
}
