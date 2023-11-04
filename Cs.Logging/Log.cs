namespace Cs.Logging;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cs.Logging.Providers;

public enum LogLevelConfig
{
    All,
    Negative,
    Error,
}

public interface ILogProvider
{
    void Info(string message);
    void Debug(string message);
    void DebugBold(string message);
    void Warn(string message);
    void Error(string message);
    [DoesNotReturn]
    void ErrorAndExit(string message);
    string BuildTag(string file, int line);
}

public static class Log
{
    private static LogLevel writeType = LogLevel.All;

    [Flags]
    public enum LogLevel
    {
        Info = 0x01,
        Debug = 0x02,
        Warn = 0x04,
        Error = 0x08,
        All = Info | Debug | Warn | Error,
    }

    public static bool WriteFileLine { get; set; } = true;
    internal static ILogProvider Provider { get; set; } = new ConsoleLogProvider();

    public static void Initialize(ILogProvider logProvider, LogLevelConfig levelConfig)
    {
        if (logProvider != null)
        {
            Provider = logProvider;
        }

        switch (levelConfig)
        {
            case LogLevelConfig.All:
                writeType = LogLevel.All;
                break;

            case LogLevelConfig.Negative:
                writeType = LogLevel.Warn | LogLevel.Error;
                break;

            case LogLevelConfig.Error:
                writeType = LogLevel.Error;
                break;
        }
    }

    public static IDisposable SwitchProvider(ILogProvider newProvider)
    {
        return new LogProviderSwitcher(newProvider);
    }

    public static void Info(string? message)
    {
        if (writeType.HasFlag(LogLevel.Info) == false)
        {
            return;
        }

        Provider.Info($"{message}");
    }

    public static void Debug(string? message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (writeType.HasFlag(LogLevel.Debug) == false)
        {
            return;
        }

        Provider.Debug(BuildMessage(message, file, line));
    }

    public static void DebugBold(string? message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (writeType.HasFlag(LogLevel.Debug) == false)
        {
            return;
        }

        Provider.DebugBold(BuildMessage(message, file, line));
    }

    public static void Warn(string? message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (writeType.HasFlag(LogLevel.Warn) == false)
        {
            return;
        }

        Provider.Warn(BuildMessage(message, file, line));
    }

    public static void Error(string? message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (writeType.HasFlag(LogLevel.Error) == false)
        {
            return;
        }

        Provider.Error($"{message} ({Provider.BuildTag(file, line)})");
    }

    public static string BuildHead(string? message)
    {
        return $"-- [{message}] --".PadRight(70, '-');
    }

    [DoesNotReturn]
    public static void ErrorAndExit(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        Provider.ErrorAndExit($"{message} ({Provider.BuildTag(file, line)})");
    }

    private static string BuildMessage(string message, string file, int line)
    {
        if (WriteFileLine)
        {
            return $"{message} ({Provider.BuildTag(file, line)})";
        }

        return message;
    }
}
