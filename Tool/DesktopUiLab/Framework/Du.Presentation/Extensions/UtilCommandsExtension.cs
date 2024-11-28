namespace Du.Presentation.Extensions;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using Shared.Templet.Errors;

public sealed class UtilCommandsExtension : MarkupExtension
{
    public UtilCommandsExtension()
    {
        this.OpenInExplorer = new RelayCommand<string>(path =>
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            // note: 절대경로여야 한다.
            if (path.Contains(":") == false)
            {
                path = Path.GetFullPath(path);
            }

            Process.Start("explorer.exe", $"/select,\"{path}\"");
        });

        this.OpenFile = new RelayCommand<string>(path =>
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            // note: 절대경로여야 한다.
            if (path.Contains(":") == false)
            {
                path = Path.GetFullPath(path);
            }

            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        });

        this.CopyToClipboard = new RelayCommand<string>(text =>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            Clipboard.SetText(text);
            Log.Info($"클립보드에 복사:\n{text}");
        });
    }

    public ICommand OpenInExplorer { get; }
    public ICommand OpenFile { get; }
    public ICommand CopyToClipboard { get; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new UtilCommandsExtension();
    }
}
