namespace Cs.Legacy
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class ServiceController
    {
        public const string ServiceInstallCmd = "ServiceInstall";
        public const string ServiceUninstallCmd = "ServiceUninstall";
        public const string ServiceStartCmd = "ServiceStart";
        public const string ServiceStopCmd = "ServiceStop";
        public const string ServiceRunCmd = "ServiceRun";

        public static bool HandleServiceCommands(string serviceName, Action<bool> onStart, Action onStop, string[] args)
        {
            if (ParseArguments(args, out NativeMethods.ServiceArgs serviceArgs) == false)
            {
                // 서비스 관련 실행이 아닌 경우 : 콘솔 핸들러에 onStop 호출을 연결한다.
                NativeMethods.consoleCtrlhandler += ctrlType =>
                {
                    onStop.Invoke();
                    return true;
                };
                NativeMethods.SetConsoleCtrlHandler(NativeMethods.consoleCtrlhandler, add: true);

                return false;
            }

            if (String.IsNullOrEmpty(serviceArgs.ServiceName) == false)
            {
                serviceName = serviceArgs.ServiceName;
            }

            switch (serviceArgs.Cmd)
            {
                case ServiceInstallCmd:
                    Install(serviceName, serviceArgs.WorkingDir);
                    return true;

                case ServiceUninstallCmd:
                    Uninstall(serviceName);
                    return true;

                case ServiceStartCmd:
                    Start(serviceName);
                    return true;

                case ServiceStopCmd:
                    Stop(serviceName);
                    return true;

                case ServiceRunCmd:
                    Run(serviceName, serviceArgs, onStart, onStop);
                    return true;
            }

            return false;
        }

        public static void Start(string serviceName)
        {
            LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Start.");

            IntPtr scm = OpenSCManager(serviceName, NativeMethods.SCMAccessRights.AllAccess);

            IntPtr service = NativeMethods.OpenService(scm, serviceName, NativeMethods.ServiceAccessRights.AllAccess);
            if (service == IntPtr.Zero)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Not Installed.");
            }

            var status = new NativeMethods.ServiceStatus();
            if (NativeMethods.QueryServiceStatus(service, status) == 1)
            {
                if (status.CurrentState != NativeMethods.ServiceState.Stopped)
                {
                    LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Not Stopped.");
                    return;
                }
            }

            if (NativeMethods.StartService(service, 0, 0) == 0)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"StartService Failed.");
            }

            Thread.Sleep(2000);

            int start = Environment.TickCount;
            while (NativeMethods.QueryServiceStatus(service, status) == 1)
            {
                if (status.CurrentState == NativeMethods.ServiceState.Running)
                {
                    LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Successfully Started.");
                    break;
                }
                else if (status.CurrentState == NativeMethods.ServiceState.Stopped)
                {
                    LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Start Failed.");
                }

                int current = Environment.TickCount;
                if (current - start >= 30000)
                {
                    LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Start Timed out.");
                }
            }

            NativeMethods.CloseServiceHandle(service);
            NativeMethods.CloseServiceHandle(scm);
        }

        public static void Stop(string serviceName)
        {
            LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Stop.");

            IntPtr scm = OpenSCManager(serviceName, NativeMethods.SCMAccessRights.AllAccess);

            IntPtr service = NativeMethods.OpenService(scm, serviceName, NativeMethods.ServiceAccessRights.AllAccess);
            if (service == IntPtr.Zero)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Not Installed.");
            }

            var status = new NativeMethods.ServiceStatus();
            if (NativeMethods.QueryServiceStatus(service, status) == 1)
            {
                if (status.CurrentState != NativeMethods.ServiceState.Running)
                {
                    LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Not Running.");
                    return;
                }
            }

            if (NativeMethods.ControlService(service, NativeMethods.ServiceControl.Stop, status) == 0)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} ControlService Failed.");
            }

            int start = Environment.TickCount;
            while (NativeMethods.QueryServiceStatus(service, status) == 1)
            {
                if (status.CurrentState == NativeMethods.ServiceState.Stopped)
                {
                    LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Successfully Stopped.");
                    break;
                }

                int current = Environment.TickCount;
                if (current - start >= 30000)
                {
                    LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Stop Timed out.");
                }
            }

            NativeMethods.CloseServiceHandle(service);
            NativeMethods.CloseServiceHandle(scm);
        }

        public static bool IsInService()
        {
            /*
             * core 에서 아직 정상 동작하지 않는다. dotnet5 에도 코드 병합이 되어 있지 않음.
             * must be Environment.UserInteractive == false
             * https://github.com/dotnet/runtime/issues/770
             * https://github.com/dotnet/core/issues/3017
            */
            ////if (Environment.UserInteractive)
            ////{
            ////    return false;
            ////}

            // 위에가 정상동작 하지 않아 일단 임시방편으로 InputRedirected 로 확인 후 처리
            return Console.IsInputRedirected;
        }

        private static void Install(string serviceName, string workingDir)
        {
            LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Install Start.");

            IntPtr scm = OpenSCManager(serviceName, NativeMethods.SCMAccessRights.AllAccess);

            IntPtr service = NativeMethods.OpenService(scm, serviceName, NativeMethods.ServiceAccessRights.AllAccess);
            if (service != IntPtr.Zero)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Already Installed.");
            }

            if (Directory.Exists(workingDir) == false)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, "Working Directory Does Not Exist.");
            }

            string binaryPathName = MakeBinaryPathName(workingDir);

            service = NativeMethods.CreateService(
                    scm,
                    serviceName,
                    serviceName,
                    NativeMethods.ServiceAccessRights.AllAccess,
                    0x00000010,
                    NativeMethods.ServiceBootFlag.AutoStart,
                    NativeMethods.ServiceError.Normal,
                    binaryPathName,
                    null,
                    IntPtr.Zero,
                    null,
                    null,
                    null);

            if (service == IntPtr.Zero)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, "CreateService Failed.");
            }

            var actions = new NativeMethods.SCAction[3];
            actions[0].Type = NativeMethods.SCActionType.Restart;
            actions[0].Delay = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            actions[1].Type = NativeMethods.SCActionType.Restart;
            actions[1].Delay = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            actions[2].Type = NativeMethods.SCActionType.Restart;
            actions[2].Delay = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

            IntPtr actionPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.SCAction)) * actions.Length);
            if (actionPtr == IntPtr.Zero)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, "SCAction Alloc Failed.");
            }

            for (int i = 0; i < actions.Length; ++i)
            {
                Marshal.StructureToPtr(actions[i], actionPtr + (Marshal.SizeOf(typeof(NativeMethods.SCAction)) * i), false);
            }

            var failureActions = new NativeMethods.ServiceFailureActions();
            failureActions.ResetPeriod = (int)TimeSpan.FromDays(1).TotalSeconds;
            failureActions.RebootMsg = string.Empty;
            failureActions.Command = string.Empty;
            failureActions.Actions = actions.Length;
            failureActions.ActionsPtr = actionPtr;

            IntPtr failureActionsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(failureActions));
            if (failureActionsPtr == IntPtr.Zero)
            {
                Marshal.FreeHGlobal(actionPtr);
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, "ServiceFailureActions Alloc Failed.");
            }

            Marshal.StructureToPtr(failureActions, failureActionsPtr, false);

            if (NativeMethods.ChangeServiceConfig2(service, NativeMethods.ServiceInfoLevel.FailureActions, failureActionsPtr) == 0)
            {
                Marshal.FreeHGlobal(failureActionsPtr);
                Marshal.FreeHGlobal(actionPtr);
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, "ChangeServiceConfig2 Failed.");
            }

            Marshal.FreeHGlobal(failureActionsPtr);
            Marshal.FreeHGlobal(actionPtr);

            LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Installed Successfully.");

            NativeMethods.CloseServiceHandle(service);
            NativeMethods.CloseServiceHandle(scm);
        }

        private static void Uninstall(string serviceName)
        {
            LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Uninstall Start.");

            IntPtr scm = OpenSCManager(serviceName, NativeMethods.SCMAccessRights.AllAccess);

            IntPtr service = NativeMethods.OpenService(scm, serviceName, NativeMethods.ServiceAccessRights.AllAccess);
            if (service == IntPtr.Zero)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Not Installed.");
            }

            if (NativeMethods.DeleteService(service) == 0)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"DeleteService Failed. LastError : {Marshal.GetLastWin32Error()}");
            }

            LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Info, $"{serviceName} Service Uninstalled Successfully.");

            NativeMethods.CloseServiceHandle(service);
            NativeMethods.CloseServiceHandle(scm);
        }

        private static void Run(string serviceName, NativeMethods.ServiceArgs serviceArgs, Action<bool> onStart, Action onStop)
        {
            if (IsInService() == false)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, $"{serviceName} Service Run Failed. (UserInteractive).");
            }

            if (Directory.Exists(serviceArgs.WorkingDir) == false)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, "Working Directory Does Not Exist.");
            }

            System.IO.Directory.SetCurrentDirectory(serviceArgs.WorkingDir);

            var builder = Host.CreateApplicationBuilder();
            builder.Services.AddWindowsService(option =>
            {
                option.ServiceName = serviceName;
            });

            builder.Services.AddHostedService<Service>(e => new Service(serviceName, onStart, onStop));

            var host = builder.Build();
            host.Run();
        }

        private static bool ParseArguments(string[] args, out NativeMethods.ServiceArgs serviceArgs)
        {
            serviceArgs = new NativeMethods.ServiceArgs();
            if (args == null || args.Length <= 0)
            {
                return false;
            }

            serviceArgs.Cmd = args[0];

            if ((serviceArgs.Cmd == ServiceInstallCmd || serviceArgs.Cmd == ServiceRunCmd) &&
                args.Length >= 2)
            {
                serviceArgs.WorkingDir = args[1];
            }

            if (args.Length >= 3)
            {
                serviceArgs.ServiceName = args[2];
            }

            return true;
        }

        private static IntPtr OpenSCManager(string serviceName, NativeMethods.SCMAccessRights scmAccessRights)
        {
            IntPtr scm = NativeMethods.OpenSCManager(null, null, scmAccessRights);
            if (scm == IntPtr.Zero)
            {
                LogAndExit(serviceName, NativeMethods.ServiceLogLevel.Error, "OpenSCManager Failed. Need to Run as Administrator");
            }

            return scm;
        }

        private static string MakeBinaryPathName(string workingDir)
        {
            // var path = $"dotnet {Assembly.GetEntryAssembly().Location} {ServiceRunCmd} {workingDir}";
            var path = $"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName} {ServiceRunCmd} {workingDir}";
            return path;
        }

        private static void LogAndExit(string serviceName, NativeMethods.ServiceLogLevel logLevel, string msg)
        {
            using (var sw = new StreamWriter($"{AppDomain.CurrentDomain.BaseDirectory}{serviceName}_ServiceLog_{DateTime.Now.ToString("yyyyMMdd")}.log", true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss,fff")} [{logLevel.ToString()}] {msg}");
            }

            if (logLevel == NativeMethods.ServiceLogLevel.Error)
            {
                throw new ApplicationException(msg);
            }
        }

        private class Service : BackgroundService
        {
            private readonly Action<bool> onStart;
            private readonly Action onStop;

            public Service(string serviceName, Action<bool> onStart, Action onStop)
            {
                this.ServiceName = serviceName;
                this.CanPauseAndContinue = false;
                this.onStart = onStart;
                this.onStop = onStop;
            }

            private Service()
            {
            }

            public string ServiceName { get; }
            public bool CanPauseAndContinue { get; }

            public override Task StopAsync(CancellationToken cancellationToken)
            {
                this.onStop.Invoke();
                return Task.CompletedTask;
            }

            protected override Task ExecuteAsync(CancellationToken stoppingToken)
            {
                this.onStart.Invoke(true);
                return Task.CompletedTask;
            }
        }
    }
}
