namespace CutEditor.Dialogs;

using System.Collections;
using System.Windows;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.Base;
using Shared.Templet.TempletTypes;
using Wpf.Ui.Controls;

public partial class UnitPickerDialog : ContentDialog
{
    private readonly IFilteredCollection filteredList;
    private string searchKeyword = string.Empty;

    public UnitPickerDialog()
    {
        this.DataContext = this;

        var units = TempletContainer<Unit>.Values.Where(e => e.EnableForCutscene()).ToArray();
        this.filteredList = App.Current.Services.GetRequiredService<IFilteredCollectionProvider>().Build(units);
        this.InitializeComponent();
    }

    public IEnumerable FilteredFiles => this.filteredList.List;
    public Unit? SelectedUnit { get; set; }
    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.UpdateSearchKeyword(value);
    }

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

    private void UpdateSearchKeyword(string value)
    {
        this.searchKeyword = value;
        this.filteredList.Refresh(this.searchKeyword);
    }
}
