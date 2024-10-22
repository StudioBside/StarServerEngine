namespace Du.WpfLib;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Cs.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

public sealed class FileAndSnackbarLog : ILogProvider, IDisposable
{
    private static readonly TimeSpan Duration = TimeSpan.FromSeconds(5);
    private readonly FileStream fileStream;
    private readonly ISnackbarService snackbarService;

    public FileAndSnackbarLog(ISnackbarService snackbarService)
    {
        this.fileStream = File.Open("log.txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        this.fileStream.Write(Encoding.UTF8.GetPreamble());

        this.snackbarService = snackbarService;
    }

    public void Dispose()
    {
        this.fileStream.Dispose();
    }

    public void Debug(string message)
    {
        this.WriteLine($"[DEBUG] {message}");
    }

    public void DebugBold(string message)
    {
        this.WriteLine($"[DEBUG] {message}");
    }

    public void Info(string message)
    {
        this.WriteLine($"[INFO] {message}");

        if (this.snackbarService.GetSnackbarPresenter() is not null)
        {
            // Info는 파란색. 시인성 위해 Secondary(흰색)
            this.snackbarService.Show("Info", message, ControlAppearance.Secondary, icon: null, Duration);
        }
    }

    public void Warn(string message)
    {
        this.WriteLine($"[WARN] {message}");
        // Caution은 주황색. 시인성 위해 Secondary(흰색)
        this.snackbarService.Show("Warn", message, ControlAppearance.Secondary, icon: null, Duration);
    }

    [DoesNotReturn]
    public void ErrorAndExit(string message)
    {
        Console.WriteLine(message);
        Process.GetCurrentProcess().Kill();
        throw new Exception(message);
    }

    public void Error(string message)
    {
        this.WriteLine($"[ERROR] {message}");
        // Danger는 붉은색. 시인성 위해 Dark(검정)
        this.snackbarService.Show("Error", message, ControlAppearance.Dark, icon: null, Duration);
    }

    string ILogProvider.BuildTag(string file, int line)
    {
        return string.Intern($"{Path.GetFileName(file)}:{line}");
    }

    //// ------------------------------------------------------------------------

    private void WriteLine(string message)
    {
        var current = DateTime.Now;
        byte[] buffer = Encoding.UTF8.GetBytes($"{current:yyyy-MM-dd HH:mm:ss.fff} {message} {Environment.NewLine}");
        this.fileStream.Write(buffer);

        this.fileStream.Flush();
    }
}