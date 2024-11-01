namespace CutEditor.Dialogs.BgFilePicker;

using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using Du.Presentation.Util;
using Shared.Interfaces;

public sealed class ElementType : ISearchable
{
    private readonly string fullPath;

    public ElementType(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            this.fullPath = string.Empty;
            this.FileNameOnly = string.Empty;
        }
        else
        {
            this.fullPath = Path.GetFullPath(fileName);
            this.FileNameOnly = Path.GetFileName(fileName);
        }

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
    public ImageSource? ImageSource { get; private set; }
    public string Category { get; init; } = string.Empty;

    public bool IsTarget(string keyword) => this.FileNameOnly.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    public override string ToString() => this.FileNameOnly;

    public void LoadImage()
    {
        this.ImageSource = ImageHelper.GetThumbnail(this.fullPath, thumbnailWidth: 100);
    }
}
