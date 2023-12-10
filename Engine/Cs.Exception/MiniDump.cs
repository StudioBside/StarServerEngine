namespace Cs.Exception
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal sealed class MiniDump
    {
        [Flags]
        public enum Typ : uint
        {
            // From dbghelp.h:
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000,
            MiniDumpWithoutAuxiliaryState = 0x00004000,
            MiniDumpWithFullAuxiliaryState = 0x00008000,
            MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
            MiniDumpIgnoreInaccessibleMemory = 0x00020000,
            MiniDumpValidTypeFlags = 0x0003ffff,
        }

        public static bool Write(string fileName)
        {
            return Write(fileName, Typ.MiniDumpWithFullMemory);
        }

        public static bool Write(string fileName, Typ dumpTyp)
        {
            bool result = false;

            NativeMethods.MiniDumpExceptionInformation exp;
            exp.ThreadId = NativeMethods.GetCurrentThreadId();
            exp.ClientPointers = false;
            exp.ExceptioonPointers = Marshal.GetExceptionPointers();

            var thread = new Thread(() =>
            {
                using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                {
                    result = NativeMethods.MiniDumpWriteDump(
                        NativeMethods.GetCurrentProcess(),
                        NativeMethods.GetCurrentProcessId(),
                        fs.SafeFileHandle.DangerousGetHandle(),
                        (uint)dumpTyp,
                        ref exp,
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
            });

            thread.Name = "DumpThread";
            thread.Start();
            thread.Join();

            return result;
        }

        internal static class NativeMethods
        {
            [DllImport(
            "dbghelp.dll",
            EntryPoint = "MiniDumpWriteDump",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
            internal static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint processId,
            IntPtr hFile,
            uint dumpType,
            ref MiniDumpExceptionInformation expParam,
            IntPtr userStreamParam,
            IntPtr callbackParam);

            [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
            internal static extern uint GetCurrentThreadId();

            [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess", ExactSpelling = true)]
            internal static extern IntPtr GetCurrentProcess();

            [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcessId", ExactSpelling = true)]
            internal static extern uint GetCurrentProcessId();

            [StructLayout(LayoutKind.Sequential, Pack = 4)] // Pack=4 is important! So it works also for x64!
            internal struct MiniDumpExceptionInformation
            {
                public uint ThreadId;
                public IntPtr ExceptioonPointers;
                [MarshalAs(UnmanagedType.Bool)]
                public bool ClientPointers;
            }
        }
    }
}