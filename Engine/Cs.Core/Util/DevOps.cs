namespace Cs.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using Cs.Logging;

    public static class DevOps
    {
        private const string Separator = "@@";
        private static readonly Lazy<BuildInfo> BuildInfoValue = new Lazy<BuildInfo>(CreateBuildInfo);

        public enum P4StreamType
        {
            Dev = 1,
            Alpha,
            Sandbox,
            Cinematic,

            Stage = 10,
            Review,
            Live,

            Main = 20,
        }

        public static BuildInfo BuildInformation => BuildInfoValue.Value;

        /// <summary>
        /// p4.exe가 설치된 곳에서만 정상적인 실행이 보장됩니다. 빌드에 포함된 정보를 얻으려면 BuildInfo를 조외하세요.
        /// </summary>
        public static bool GetStreamInfoForDev(out StreamInfo result)
        {
            var temp = GetStreamInfoForDev();
            if (temp.HasValue == false)
            {
                result = default;
                return false;
            }

            result = temp.Value;
            return true;
        }

        public static bool IsRunningInLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static bool IsRunningInWsl()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            try
            {
                // /proc/version 파일 읽기
                string procVersion = File.ReadAllText("/proc/version");

                // WSL과 관련된 문자열 확인
                return procVersion.Contains("Microsoft") || procVersion.Contains("WSL");
            }
            catch (Exception ex)
            {
                // /proc/version 파일이 없거나 다른 문제가 발생한 경우
                Console.WriteLine($"Error checking WSL environment: {ex.Message}");
                return false;
            }
        }

        public static bool IsRunningInContainer()
        {
            return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        }

        //// -----------------------------------------------------------------------------------------------

        private static StreamInfo? GetStreamInfoForDev()
        {
            var host = Dns.GetHostName();
            if (string.IsNullOrEmpty(host))
            {
                Log.Error($"get hostname failed.");
                return null;
            }

            if (OutProcess.Run("p4.exe", $"-F \"%Stream%{Separator}%Host%{Separator}%Root%\" -ztag clients", out string p4Output) == false)
            {
                Log.Error($"running p4 process failed.");
                return null;
            }

            //p4 워크스페이스 정보 가공
            var clientList = new Dictionary<string, List<P4StreamRoot>>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in p4Output.Split(Environment.NewLine))
            {
                var tokens = line.Split(Separator, 3, StringSplitOptions.None); // count=2이면 첫 번째 공백에서만 잘라줌. 빈 토큰은 알아서 정리.
                if (tokens.Length < 3)
                {
                    continue;
                }

                string outputStream = tokens[0];
                string outputHost = tokens[1].ToLower();
                string outputRoot = tokens[2].ToLower();

                if (clientList.TryGetValue(outputHost, out var outputList) == false)
                {
                    outputList = new List<P4StreamRoot>();
                    clientList.Add(outputHost, outputList);
                }

                outputList.Add(new P4StreamRoot(outputStream, outputRoot));
            }

            //host 기반 스트림 검색
            string workingDirectory = System.IO.Directory.GetCurrentDirectory().ToLower();
            string? streamName = null;
            if (clientList.TryGetValue(host, out var list) == true)
            {
                foreach (var data in list)
                {
                    if (workingDirectory.StartsWith(data.Root))
                    {
                        var match = Regex.Match(data.Stream, @"//stream/(\w+)");
                        if (match.Success)
                        {
                            streamName = match.Groups[1].Value;
                            break;
                        }
                    }
                }
            }

            //모든 데이터에서 스트림검색
            if (string.IsNullOrWhiteSpace(streamName))
            {
                foreach (var data in clientList.SelectMany(e => e.Value))
                {
                    if (workingDirectory.StartsWith(data.Root))
                    {
                        var match = Regex.Match(data.Stream, @"//stream/(\w+)");
                        if (match.Success)
                        {
                            streamName = match.Groups[1].Value;
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(streamName))
            {
                Log.Error($"matched client not found. #target clients:{clientList.Count}");
                return null;
            }

            if (!Enum.TryParse<P4StreamType>(streamName, ignoreCase: true, out var streamType))
            {
                if (streamName.StartsWith("dev"))
                {
                    streamType = P4StreamType.Dev;
                }
                else if (streamName.StartsWith("alpha"))
                {
                    streamType = P4StreamType.Alpha;
                }
                else
                {
                    Log.Error($"not registered stream name:{streamName}");
                    return null;
                }
            }

            return new StreamInfo((int)streamType, streamType.ToString());
        }

        private static BuildInfo CreateBuildInfo()
        {
            var entryAssembly = Assembly.GetEntryAssembly(); // note: 실행파일의 어셈블리를 획득.
            if (entryAssembly == null)
            {
                throw new Exception($"[BuildInfo] get entryAssembly failed.");
            }

            DateTime buildDate = LinkerTime.GetBuildDate(entryAssembly);
            var assemblyName = entryAssembly.GetName();
            if (assemblyName.Name == null)
            {
                throw new Exception($"[BuildInfo] get assemblyName.Name failed.");
            }

            int revision = assemblyName.Version.GetCustomRevision();
            sbyte streamId = (sbyte)assemblyName.Version.GetStreamId();
            int protocol = assemblyName.Version.GetProtocolVersion();

            string streamName = "sourceBuild";
            if (protocol > 0)
            {
                streamName = ((P4StreamType)streamId).ToString();
            }
            else
            {
                StreamInfo? streamInfo = GetStreamInfoForDev();
                if (streamInfo != null)
                {
                    streamName = streamInfo.Value.Name;
                }
            }

            return new BuildInfo(assemblyName.Name, buildDate, revision, streamId, streamName, protocol);
        }

        public readonly struct StreamInfo
        {
            public StreamInfo(int id, string name)
            {
                this.Name = name;
                this.Id = id;
            }

            public string Name { get; }
            public int Id { get; }
            public string NameSensitive => this.Name[0].ToString().ToLower() + this.Name[1..];
        }

        public readonly struct BuildInfo
        {
            internal BuildInfo(string assemblyName, DateTime buildTime, int revision, sbyte streamId, string streamName, int protocol)
            {
                this.AssemblyName = assemblyName;
                this.BuildTime = buildTime;
                this.Revision = revision;
                this.StreamId = streamId;
                this.StreamName = streamName;
                this.Protocol = protocol;
            }

            public string AssemblyName { get; }
            public DateTime BuildTime { get; }
            public int Revision { get; }
            public sbyte StreamId { get; }
            public string StreamName { get; }
            public int Protocol { get; }
            public string BuildVersion => $"{this.StreamId}.{this.Revision}.{this.Protocol}";
        }

        private readonly struct P4StreamRoot
        {
            public P4StreamRoot(string stream, string root)
            {
                this.Stream = stream;
                this.Root = root;
            }

            public string Stream { get; }
            public string Root { get; }
        }
    }
}
