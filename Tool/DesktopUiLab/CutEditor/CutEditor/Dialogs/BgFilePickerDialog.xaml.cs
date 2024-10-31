namespace CutEditor.Dialogs;

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Wpf.Ui.Controls;

public partial class BgFilePickerDialog : ContentDialog
{
    private readonly IFilteredCollection imageFiles;
    private readonly IFilteredCollection movFiles;
    private readonly IFilteredCollection slateFiles;
    private readonly IFilteredCollection spineFiles;
    private string searchKeyword = string.Empty;

    public BgFilePickerDialog(
        string title,
        ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        this.DataContext = this;

        this.Title = title;
        var filteredCollectionProvider = App.Current.Services.GetRequiredService<IFilteredCollectionProvider>();

        this.imageFiles = filteredCollectionProvider.Build(AssetList.Instance.BgImageFiles.Select(e => new ElementType(e)));
        this.movFiles = filteredCollectionProvider.Build(AssetList.Instance.MovFiles.Select(e => new ElementType(e)));
        this.slateFiles = filteredCollectionProvider.Build(AssetList.Instance.SlateFiles.Select(e => new ElementType(e)));
        this.spineFiles = filteredCollectionProvider.Build(AssetList.Instance.SpineFiles.Select(e => new ElementType(e)));

        this.InitializeComponent();
    }

    public IEnumerable ImageFiles => this.imageFiles.List;
    public int ImageCount => this.imageFiles.SourceCount;
    public IEnumerable MovFiles => this.movFiles.List;
    public int MovCount => this.movFiles.SourceCount;
    public IEnumerable SlateFiles => this.slateFiles.List;
    public int SlateCount => this.slateFiles.SourceCount;
    public IEnumerable SpineFiles => this.spineFiles.List;
    public int SpineCount => this.spineFiles.SourceCount;
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
        this.imageFiles.Refresh(this.searchKeyword);
    }
  
    public sealed class ElementType : ISearchable
    {
        private readonly string fullPath;

        public ElementType(string fileName)
        {
            this.fullPath = Path.GetFullPath(fileName);
            this.FileNameOnly = Path.GetFileName(fileName);
            this.OpenFileCommand = new RelayCommand(() =>
            {
                Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
            });
            this.OpenInExplorerCommand = new RelayCommand(() =>
            {
                Process.Start("explorer.exe", $"/select,\"{this.fullPath}\"");
            });
        }

        public string FileNameOnly { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand OpenInExplorerCommand { get; }

        public bool IsTarget(string keyword) => this.FileNameOnly.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        public override string ToString() => this.FileNameOnly;
    }
}
