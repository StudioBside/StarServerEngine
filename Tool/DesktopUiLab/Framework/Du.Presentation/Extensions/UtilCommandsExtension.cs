namespace Du.Presentation.Extensions;

using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public sealed class UtilCommandsExtension : MarkupExtension
{
    public UtilCommandsExtension(ICommand openInExplorer, ICommand openFile, ICommand copyToClipboard)
    {
        this.OpenInExplorer = openInExplorer;
        this.OpenFile = openFile;
        this.CopyToClipboard = copyToClipboard;
    }

    // note: for reflection
    public UtilCommandsExtension()
    {
    }

    public ICommand? OpenInExplorer { get; }
    public ICommand? OpenFile { get; }
    public ICommand? CopyToClipboard { get; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var openInExplorer = new RelayCommand<string>(path =>
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            // note: 절대경로여야 한다.
            if (path.Contains(':') == false)
            {
                path = Path.GetFullPath(path);
            }

            if (Directory.Exists(path))
            {
                // 디렉토리인 경우: 탐색기에서 해당 경로가 열린 상태로 보여준다.
                Process.Start("explorer.exe", path);
                return;
            }

            Process.Start("explorer.exe", $"/select,\"{path}\"");
        });

        var openFile = new RelayCommand<string>(path =>
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            // note: 절대경로여야 한다.
            if (path.Contains(':') == false)
            {
                path = Path.GetFullPath(path);
            }

            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        });

        var copyToClipboard = new RelayCommand<string>(text =>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var clipboard = serviceProvider.GetService<IClipboardWrapper>();
            if (clipboard is null)
            {
                Log.Warn($"클립보드 서비스가 없습니다.");
                return;
            }

            clipboard.SetText(text);
            Log.Info($"클립보드에 복사:\n{text}");
        });

        return new UtilCommandsExtension(openInExplorer, openFile, copyToClipboard);
    }
}
