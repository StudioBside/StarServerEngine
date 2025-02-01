namespace Cs.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
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
            var p4ClientInfo = GetP4ClientInfoForDev();
            if (p4ClientInfo is null)
            {
                result = default;
                return false;
            }

            result = p4ClientInfo.StreamInfo;
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

        public static P4ClientInfo? GetP4ClientInfoForDev()
        {
            var host = Dns.GetHostName();
            if (string.IsNullOrEmpty(host))
            {
                Log.Error($"get hostname failed.");
                return null;
            }

            Dictionary<string/*host*/, P4ClientInfo[]> clientsByHost;
            clientsByHost = GetP4DataForDev(new[] { "Host", "Client", "Root", "Stream" }, "clients")
                .GroupBy(e => e[0])
                .ToDictionary(
                    keySelector: e => e.Key,
                    elementSelector: e => e.Select(arr => new P4ClientInfo(arr[1], arr[2], arr[3])).ToArray());
            if (clientsByHost.Count == 0)
            {
                return null;
            }

            // workspace에 현재 실행 머신의 host 전용으로 설정된 정보가 있다면 여기서 먼저 탐색.
            string workingDirectory = Directory.GetCurrentDirectory();
            P4ClientInfo? p4Client = null;
            if (clientsByHost.TryGetValue(host, out var clients))
            {
                p4Client = clients
                    .FirstOrDefault(e => workingDirectory.StartsWith(e.Root, StringComparison.OrdinalIgnoreCase));
            }

            // host 기반으로 찾지 못했다면 모든 데이터에서 스트림 검색
            if (p4Client is null)
            {
                p4Client = clientsByHost.SelectMany(e => e.Value)
                    .FirstOrDefault(e => workingDirectory.StartsWith(e.Root, StringComparison.OrdinalIgnoreCase));
            }

            if (p4Client is null)
            {
                Log.Error($"matched client not found. #target clients:{clientsByHost.Sum(e => e.Value.Length)}");
                return null;
            }

            string streamLiteral = p4Client.StreamLiteral; // 리터럴은 //stream/... 형태로 되어있음.

            // 가상 스트림인 경우 실제 스트림을 찾아 변경
            Dictionary<string /*stream*/, (string Type, string Parent)> streamData;
            streamData = GetP4DataForDev(new[] { "Stream", "Type", "Parent" }, "streams")
                .ToDictionary(e => e[0], e => (Type: e[1], Parent: e[2]));

            while (streamData.TryGetValue(streamLiteral, out var streamInfo))
            {
                if (streamInfo.Type == "virtual")
                {
                    streamLiteral = streamInfo.Parent;
                }
                else
                {
                    break;
                }
            }

            string streamName = streamLiteral.Replace("//stream/", string.Empty); // 이름만 남긴다.
            if (!Enum.TryParse<P4StreamType>(streamName, ignoreCase: true, out var streamType))
            {
                Log.Error($"not registered stream name:{streamName}");
                return null;
            }

            p4Client.StreamInfo = new StreamInfo((int)streamType, streamType.ToString());
            return p4Client;
        }

        //// -----------------------------------------------------------------------------------------------

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
                var p4ClientInfo = GetP4ClientInfoForDev();
                if (p4ClientInfo is not null)
                {
                    streamName = p4ClientInfo.StreamInfo.Name;
                }
            }

            return new BuildInfo(assemblyName.Name, buildDate, revision, streamId, streamName, protocol);
        }

        /// <summary>
        /// 변수를 %로 감싸고, 구분자(@@)로 합쳐서 인자를 만들어냅니다.
        /// 형태) -F "%{변수1}%@@%{변수2}%@@%{변수3}%" -ztag {command}
        /// 예시) Stream, Host, Root를 가져오는 경우: -F "%Stream%@@%Host%@@%Root%" -ztag clients.
        /// </summary>
        private static IReadOnlyList<string[]> GetP4DataForDev(string[] variables, string command)
        {
            string p4Command = IsRunningInLinux() ? "p4.exe" : "p4";
            var args = $"-F \"{string.Join(Separator, variables.Select(e => $"%{e}%"))}\" -ztag {command}";
            if (OutProcess.Run(p4Command, args, out string p4Output) == false)
            {
                Log.Error($"running p4 process failed.");
                return Array.Empty<string[]>();
            }

            var result = new List<string[]>();
            foreach (var line in p4Output.Split(Environment.NewLine))
            {
                var tokens = line.Split(Separator, variables.Length, StringSplitOptions.None);
                if (tokens.Length < variables.Length)
                {
                    continue;
                }

                result.Add(tokens);
            }

            return result;
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

        public sealed class P4ClientInfo
        {
            public P4ClientInfo(string client, string root, string streamLiteral)
            {
                this.Client = client;
                this.Root = root;
                this.StreamLiteral = streamLiteral;
            }

            public string Client { get; }
            public string Root { get; }
            public string StreamLiteral { get; }

            public StreamInfo StreamInfo { get; internal set; }
        }
    }
}
