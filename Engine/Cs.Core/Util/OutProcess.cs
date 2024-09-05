namespace Cs.Core.Util
{
    using System;
    using System.Diagnostics;
    using System.Text;

    public sealed class OutProcess : IDisposable
    {
        private readonly Process process = new Process();
        private readonly StringBuilder error = new StringBuilder();

        private OutProcess()
        {
            this.process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    this.error.AppendLine(args.Data);
                }
            };
        }

        public static bool Run(string processName, string args)
        {
            using var outProcess = new OutProcess();
            try
            {
                return outProcess.Execute(processName, args, out string result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool Run(string processName, string args, out string result)
        {
            using var outProcess = new OutProcess();
            try
            {
                return outProcess.Execute(processName, args, out result);
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return false;
            }
        }

        public void Dispose()
        {
            this.process.Dispose();
        }

        private bool Execute(string processName, string args, out string result)
        {
            var psi = new ProcessStartInfo(processName, args)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            this.process.StartInfo = psi;
            this.process.Start();

            // deadlock을 피하기 위해 대기 순서가 중요하다.
            // https://docs.microsoft.com/ko-kr/dotnet/api/system.diagnostics.processstartinfo.redirectstandarderror?view=netframework-4.7.2
            this.process.BeginErrorReadLine();
            result = this.process.StandardOutput.ReadToEnd();
            this.process.WaitForExit();

            string errorMessage = this.error.ToString();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                result = errorMessage;
                return false;
            }

            // note: 에러로그가 없는 프로세스의 정상 종료 여부를 확인하도록 합니다.
            return this.process.ExitCode == 0;
        }
    }
}
