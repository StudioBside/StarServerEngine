namespace Cs.Legacy
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        internal delegate bool ConsoleCtrlHandler(CtrlType sig);

        [Flags]
        internal enum SCMAccessRights
        {
            Connect = 0x0001,
            CreateService = 0x0002,
            EnumerateService = 0x0004,
            Lock = 0x0008,
            QueryLockStatus = 0x0010,
            ModifyBootConfig = 0x0020,
            StandardRightsRequired = 0xF0000,
            AllAccess = StandardRightsRequired | Connect | CreateService | EnumerateService | Lock | QueryLockStatus | ModifyBootConfig,
        }

        [Flags]
        internal enum ServiceAccessRights
        {
            QueryConfig = 0x0001,
            ChangeConfig = 0x0002,
            QueryStatus = 0x0004,
            EnumerateDependants = 0x0008,
            Start = 0x0010,
            Stop = 0x0020,
            PauseContinue = 0x0040,
            Interrogate = 0x0080,
            UserDefinedControl = 0x0100,
            StandardRightsRequired = 0xF0000,
            AllAccess = StandardRightsRequired | QueryConfig | ChangeConfig | QueryStatus | EnumerateDependants | Start | Stop | PauseContinue | Interrogate | UserDefinedControl,
        }

        internal enum ServiceBootFlag
        {
            Start = 0x00000000,
            SystemStart = 0x00000001,
            AutoStart = 0x00000002,
            DemandStart = 0x00000003,
            Disabled = 0x00000004,
        }

        internal enum ServiceState
        {
            Stopped = 0x00000001,
            StartPending = 0x00000002,
            StopPending = 0x00000003,
            Running = 0x00000004,
            ContinuePending = 0x00000005,
            PausePending = 0x00000006,
            Paused = 0x00000007,
        }

        internal enum ServiceControl
        {
            Stop = 0x00000001,
            Pause = 0x00000002,
            Continue = 0x00000003,
            Interrogate = 0x00000004,
            Shutdown = 0x00000005,
            ParamChange = 0x00000006,
            NetBindAdd = 0x00000007,
            NetBindRemove = 0x00000008,
            NetBindEnable = 0x00000009,
            NetBindDisable = 0x0000000A,
        }

        internal enum ServiceError
        {
            Ignore = 0x00000000,
            Normal = 0x00000001,
            Severe = 0x00000002,
            Critical = 0x00000003,
        }

        internal enum ServiceInfoLevel
        {
            Description = 1,
            FailureActions = 2,
            DelayedAutoStartInfo = 3,
            FailureActionsFlag = 4,
            ServiceSIDInfo = 5,
            RequiredPrivilegesInfo = 6,
            PreShutdownInfo = 7,
            TriggerInfo = 8,
            PreferredNode = 9,
            LaunchProtected = 12,
        }

        internal enum SCActionType
        {
            None = 0,
            Restart = 1,
            Reboot = 2,
            RunCommand = 3,
        }

        internal enum ServiceLogLevel
        {
            Info,
            Warn,
            Error,
        }

        internal enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }

        internal static ConsoleCtrlHandler consoleCtrlhandler { get; set; }

        [DllImport("Kernel32")]
        internal static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandler handler, bool add);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, SCMAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceAccessRights dwDesiredAccess, int dwServiceType, ServiceBootFlag dwStartType, ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll")]
        internal static extern int QueryServiceStatus(IntPtr hService, ServiceStatus lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int DeleteService(IntPtr hService);

        [DllImport("advapi32.dll")]
        internal static extern int ControlService(IntPtr hService, ServiceControl dwControl, ServiceStatus lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int ChangeServiceConfig2(IntPtr hService, ServiceInfoLevel dwInfoLevel, IntPtr lpInfo);

        [StructLayout(LayoutKind.Sequential)]
        internal struct ServiceFailureActions
        {
            public int ResetPeriod;
            public string RebootMsg;
            public string Command;
            public int Actions;
            public IntPtr ActionsPtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCAction
        {
            public SCActionType Type;
            public int Delay;
        }

        internal struct ServiceArgs
        {
            public string Cmd;
            public string WorkingDir;
            public string ServiceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class ServiceStatus
        {
            public int ServiceType { get; set; }
            public ServiceState CurrentState { get; set; }
            public int ControlsAccepted { get; set; }
            public int Win32ExitCode { get; set; }
            public int ServiceSpecificExitCode { get; set; }
            public int CheckPoint { get; set; }
            public int WaitHint { get; set; }
        }
    }
}
