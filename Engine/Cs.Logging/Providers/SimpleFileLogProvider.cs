﻿namespace Cs.Logging.Providers;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Cs.Logging.Detail;
using static Cs.Logging.Log;

public sealed class SimpleFileLogProvider : ILogProvider, IDisposable
{
    private readonly FileStream fileStream;

    public SimpleFileLogProvider(string fileName)
    {
        this.fileStream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        this.fileStream.Write(Encoding.UTF8.GetPreamble());
    }

    public SimpleFileLogProvider(string fileName, bool writeToConsole) : this(fileName)
    {
        this.WriteToConsole = writeToConsole;
    }

    public bool WriteToConsole { get; private set; }

    public void Dispose()
    {
        this.fileStream.Dispose();
    }

    public void Debug(string message)
    {
        this.WriteLine($"[DEBUG] {message}");

        if (this.WriteToConsole)
        {
            ConsoleWriter.PutLog(LogLevel.Debug, message);
        }
    }

    public void DebugBold(string message)
    {
        this.WriteLine($"[DEBUG] {message}");

        if (this.WriteToConsole)
        {
            ConsoleWriter.PutLog(LogLevel.Debug, message, ConsoleColor.Cyan);
        }
    }

    public void Info(string message)
    {
        this.WriteLine($"[INFO] {message}");

        if (this.WriteToConsole)
        {
            ConsoleWriter.PutLog(LogLevel.Info, message);
        }
    }

    public void Warn(string message)
    {
        this.WriteLine($"[WARN] {message}");

        if (this.WriteToConsole)
        {
            ConsoleWriter.PutLog(LogLevel.Warn, message);
        }
    }

    public void SetWriteToConsole(bool value)
    {
        this.WriteToConsole = value;
    }

    [DoesNotReturn]
    public void ErrorAndExit(string message)
    {
        Console.WriteLine(message);

        if (string.IsNullOrEmpty(this.fileStream.Name) == false && Console.IsInputRedirected == false)
        {
            var psi = new ProcessStartInfo
            {
                FileName = this.fileStream.Name,
                UseShellExecute = true,
            };

            Process.Start(psi);
        }

        Process.GetCurrentProcess().Kill();
        throw new Exception(message);
    }

    public void Error(string message)
    {
        this.WriteLine($"[ERROR] {message}");

        if (this.WriteToConsole)
        {
            ConsoleWriter.PutLog(LogLevel.Error, message);
        }
    }

    public string BuildTag(string file, int line)
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