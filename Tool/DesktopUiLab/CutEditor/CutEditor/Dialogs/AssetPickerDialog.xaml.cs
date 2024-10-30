namespace CutEditor.Dialogs;

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NPOI.HPSF;
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
    public ElementType? Selected { get; set; }
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
  
    public sealed class ElementType : ISearchable
    {
        private readonly string fullPath;

        public ElementType(string fileName)
        {
            this.fullPath = Path.GetFullPath(fileName);
            this.FileName = fileName;
            this.FileNameOnly = Path.GetFileName(fileName);
            this.OpenFileCommand = new RelayCommand(() =>
            {
                Process.Start(new ProcessStartInfo(this.FileName) { UseShellExecute = true });
            });
            this.OpenInExplorerCommand = new RelayCommand(() =>
            {
                Process.Start("explorer.exe", $"/select,\"{this.fullPath}\"");
            });
        }

        public string FileName { get; }
        public string FileNameOnly { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand OpenInExplorerCommand { get; }

        public bool IsTarget(string keyword) => this.FileName.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }
}
