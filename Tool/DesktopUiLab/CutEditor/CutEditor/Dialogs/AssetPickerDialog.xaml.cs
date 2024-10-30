namespace CutEditor.Dialogs;

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Wpf.Ui.Controls;

public partial class AssetPickerDialog : ContentDialog
{
    private readonly IFilteredCollection filteredList;
    private string searchKeyword = string.Empty;

    public AssetPickerDialog(
        string title,
        IEnumerable<string> sourceList,
        ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        this.DataContext = this;

        var elements = sourceList.Select(e => new ElementType(e)).ToArray();
        this.Title = $"{title} ({elements.Length} files)";
        this.filteredList = App.Current.Services.GetRequiredService<IFilteredCollectionProvider>().Build(elements);
        this.InitializeComponent();
    }

    public IEnumerable FilteredFiles => this.filteredList.List;
    public string? Selected { get; set; }
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

    private sealed record ElementType(string FileName) : ISearchable
    {
        public bool IsTarget(string keyword) => this.FileName.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }
}
