namespace Cs.Exception
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Cs.Core.Util;
    using Cs.Logging;

    public interface ISlackSender
    {
        void SendSnippet(string title, string text);
        void SendMessage(string userName, string authorName, string title, string text);
    }

    public sealed class ExceptionHandler
    {
        private static readonly string Hostname = Dns.GetHostName();

        private static volatile int handled = 0;

        private static string crashTarget = null!;
        private static CrashConfig crashConfig = null!;
        private static ISlackSender slackSender = null!;
        private static IDisposable sentryResource = null!;
        private static IReadOnlyList<Type> ignoreUnobservedTypes = null!;

        public static void Initialize(CrashConfig config, string crashTarget)
        {
            ExceptionHandler.crashConfig = config;
            ExceptionHandler.crashTarget = crashTarget;

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            TaskScheduler.UnobservedTaskException += UnobserveredTaskExceptionHandler;
        }

        public static void InitSlackSender(ISlackSender slackSender)
        {
            ExceptionHandler.slackSender = slackSender;
        }

        public static void SetIgnoreUnobservedTypes(params Type[] types)
        {
            ignoreUnobservedTypes = types;
        }

        public static bool Crash(Exception e)
        {
            if (crashConfig == null)
            {
                return false;
            }

            HandleException(e);
            return true;
        }

        public static bool ReportOnly(Exception e)
        {
            if (crashConfig.IgnoreReportOnly)
            {
                return Crash(e);
            }

            try
            {
                Log.Error(ExceptionUtil.FlattenInnerExceptions(e));

                if (Debugger.IsAttached)
                {
                    System.Diagnostics.Trace.WriteLine(e.ToString());
                    Debugger.Break();
                }

#if !DEBUG
                SendCrashSlack(e);
#endif
            }
            catch (Exception localException)
            {
                Log.Error(localException.Message);
            }

            return true;
        }

        public static void SendSlackWarning(string message)
        {
            if (slackSender == null)
            {
                return;
            }

            var buildInfo = DevOps.BuildInformation;
            var authorName = $"stream:{buildInfo.StreamName} revision:{buildInfo.Revision}";

            Log.Warn($"[SlackWarning] {authorName} {message}");
            slackSender.SendMessage("Slack Warning", authorName, $"[{Hostname}] {crashTarget}", message);
        }

        private static bool GuardExceptionHandling()
        {
            if (Interlocked.CompareExchange(ref handled, 1, 0) != 0)
            {
                return false;
            }

            return true;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            HandleException((Exception)args.ExceptionObject);
        }

        private static void UnobserveredTaskExceptionHandler(object? sender, UnobservedTaskExceptionEventArgs args)
        {
            if (ignoreUnobservedTypes != null)
            {
                foreach (var type in ignoreUnobservedTypes)
                {
                    if (ExceptionUtil.HasException(args.Exception, type))
                    {
                        args.SetObserved();
                        var exceptionString = ExceptionUtil.FlattenInnerExceptions(args.Exception);
                        Log.Warn($"[Exception] Ignore unobserved exception. type:{type.Name} exception:{exceptionString}");
                        return;
                    }
                }
            }

            var exception = args.Exception?.InnerException;
            if (exception == null)
            {
                args.SetObserved();
                SendSlackWarning($"[Exception] unobservedTask exception has no exception instance.");
                return;
            }

            HandleException(exception);
        }

        private static void MakeDump(Exception e)
        {
            string date = ServiceTime.NowFileString;
            string dumpFileName = $"{crashTarget}_{date}_{Hostname}.dmp";
            MiniDump.Write(dumpFileName);
        }

        private static void SendCrashSlack(Exception ex)
        {
            if (slackSender == null)
            {
                return;
            }

            Log.Info("Sending crash slack...");

            // send slack
            slackSender.SendSnippet($"{crashTarget} Crash", BuildMessage(ex, includeSummary: true));
        }

        private static void HandleException(Exception e)
        {
            if (GuardExceptionHandling() == false)
            {
                Thread.Sleep(Timeout.Infinite);
                return;
            }

            try
            {
                Log.Error(ExceptionUtil.FlattenInnerExceptions(e));

                if (Debugger.IsAttached)
                {
                    System.Diagnostics.Trace.WriteLine(e.ToString());
                    Debugger.Break();
                }

                if (Thread.CurrentThread.Name == null)
                {
                    Thread.CurrentThread.Name = "CrashThread";
                }

                if (crashConfig.MakeDump)
                {
                    MakeDump(e);
                }

#if !DEBUG
                SendCrashSlack(e);
#endif
            }
            catch (Exception localException)
            {
                Log.Error(localException.Message);
            }
            finally
            {
                sentryResource?.Dispose();

                // terminating app
                Environment.Exit(-2);
            }
        }

        private static string BuildSummary(Exception ex)
        {
            var buildInfo = DevOps.BuildInformation;
            return $"[{Hostname}] {crashTarget} Crash (streamId:{buildInfo.StreamName} revision:{buildInfo.Revision} buildDate:{buildInfo.BuildTime})";
        }

        private static string BuildMessage(Exception ex, bool includeSummary)
        {
            var buildInfo = DevOps.BuildInformation;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                if (includeSummary)
                {
                    writer.WriteLine(BuildSummary(ex));
                    writer.WriteLine();
                }

                writer.WriteLine($"Build date : {buildInfo.BuildTime}");
                writer.WriteLine($"Server Start At : {Process.GetCurrentProcess().StartTime}");
                writer.WriteLine($"Crashed At : {ServiceTime.Now.ToString()}");
                writer.WriteLine($"Build stream : {buildInfo.StreamName}");
                writer.WriteLine($"Build revision : {buildInfo.Revision}");
                writer.WriteLine($"Protocol Version : {buildInfo.Protocol}");

                writer.WriteLine("-----------------------------------------------------------------------");
                ExceptionUtil.FlattenInnerExceptions(ex, writer);
                writer.WriteLine("-----------------------------------------------------------------------");

                var stackTrace = Environment.StackTrace;
                writer.WriteLine("Stack Trace : ");
                writer.WriteLine(stackTrace);

                return writer.ToString();
            }
        }

        public sealed class CrashConfig
        {
            public bool MakeDump { get; set; }
            public bool IgnoreReportOnly { get; set; }
        }
    }
}
