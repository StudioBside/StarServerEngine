namespace CutEditor.Dialogs;

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NKM;
using Shared.Templet.Base;
using Shared.Templet.TempletTypes;
using Wpf.Ui.Controls;

public partial class ArcpointPickerDialog : ContentDialog
{
    private readonly ISearchableCollection<LobbyItem> filteredList;
    private string searchKeyword = string.Empty;

    public ArcpointPickerDialog(ContentPresenter? dialogHost) : base(dialogHost)
    {
        this.DataContext = this;

        var list = TempletContainer<LobbyItem>.Values.Where(e => e.SubType == NormalItemSubType.STN_ARCPOINT);
        this.filteredList = App.Current.Services.GetRequiredService<ISearchableCollectionProvider>().Build(list);
        this.InitializeComponent();
    }

    public IEnumerable FilteredFiles => this.filteredList.List;
    public LobbyItem? Selected { get; set; }
    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.UpdateSearchKeyword(value);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.Selected is null)
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
