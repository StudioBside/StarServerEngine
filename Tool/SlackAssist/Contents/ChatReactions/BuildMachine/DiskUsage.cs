namespace SlackAssist.Contents.ChatReactions.BuildMachine;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs.Core.Util;
using Cs.Logging;
using SlackAssist.Fremawork.Slack;
using SlackAssist.Fremawork.Slack.ChatReactionBase;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;

internal sealed class DiskUsage : StringContainsReaction
{
    public override string Trigger => "[빌드머신 디스크]";
    private string DebugName => $"[DiskUsage]";

    public override Block GetIntroduceBlock()
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"트리거 : `{this.Trigger}`");
            writer.WriteLine($"효과 : 클라이언트 빌드 머신의 디스크 사용 현황을 표시합니다.");
        }

        return new SectionBlock { Text = new Markdown(sb.ToString()) };
    }

    public override async Task Process(ISlackApiClient slack, TargetEventRecord eventRecord)
    {
        var targetEvent = eventRecord.TargetEvent;
        string userName = await targetEvent.GetUserNameAsync(slack);
        string channelName = await eventRecord.OriginalEvent.GetChannelNameAsync(slack);

        Log.Debug($"{this.DebugName} message:{targetEvent.Text} userName:{userName} channelName:{channelName}");

        if (GetDiskUsage(out var usageInfos) == false)
        {
            await slack.Chat.PostMessage(new Message
            {
                Text = "디스크 사용량 조회에 실패했습니다.",
                Channel = eventRecord.OriginalEvent.Channel,
                ThreadTs = targetEvent.Ts,
            });
            return;
        }

        var message = ConvertToSlackMessage(usageInfos);
        message.Channel = eventRecord.OriginalEvent.Channel;
        message.ThreadTs = targetEvent.Ts;

        await slack.Chat.PostMessage(message);
    }

    //// ------------------------------------------------------------------------------------------

    private static Message ConvertToSlackMessage(IReadOnlyList<DiskUsageInfo> diskUsageInfos)
    {
        var message = new Message
        {
            IconEmoji = ":apple-logo:",
            Username = "Tim Cook",
        };

        using var builder = new MarkdownBuilder();
        builder.Write($"빌드머신 디스크 사용량");
        message.Blocks.Add(builder.FlushToSectionBlock());

        foreach (var diskByServer in diskUsageInfos.GroupBy(e => e.Server))
        {
            var serverName = diskByServer.Key
                .Split('@').Last() // 앞부분에 계정 정보 잘라낸다.
                .Split('.').First(); // 뒷부분에 도메인 정보 잘라낸다.

            builder.WriteBoldLine(serverName);
            
            foreach (var disk in diskByServer)
            {
                builder.WriteLine($"{disk.DiskType} {disk.UsedSize} / {disk.TotalSize} ({disk.UsageRate}) {disk.GetProgressBar()} {disk.AvailableSize} free");
           }

            message.Blocks.Add(builder.FlushToSectionBlock());
        }

        return message;
    }

    private static bool GetDiskUsage([MaybeNullWhen(false)] out IReadOnlyList<DiskUsageInfo> output)
    {
        IReadOnlyList<string> serverList = [
            "buildman@ST-ClientBuild01.bside.com",
            "buildman@ST-ClientBuild02.bside.com",
            "buildman@ST-ClientBuild03.bside.com",
            "buildman@ST-ClientBuild04.bside.com",
            "buildman@ST-ClientBuild05.bside.com",
            "buildman@ST-ClientBuild07.bside.com",
            "buildman@ST-ClientBuild08.bside.com",
            "buildman@ST-ClientBuild09.bside.com",
            "buildman@ST-ClientBuild10.bside.com",
        ];

        /*
        ➜ df -h
        Filesystem                              Size    Used   Avail Capacity iused ifree %iused  Mounted on
        /dev/disk3s1s1                         926Gi    19Gi   475Gi     4%    393k  4.3G    0%   /
        devfs                                  207Ki   207Ki     0Bi   100%     717     0  100%   /dev
        /dev/disk3s6                           926Gi   3.0Gi   475Gi     1%       3  5.0G    0%   /System/Volumes/VM
        /dev/disk3s2                           926Gi    11Gi   475Gi     3%    1.5k  5.0G    0%   /System/Volumes/Preboot
        /dev/disk3s4                           926Gi   603Mi   475Gi     1%     254  5.0G    0%   /System/Volumes/Update
        /dev/disk1s2                           500Mi   6.0Mi   483Mi     2%       1  4.9M    0%   /System/Volumes/xarts
        /dev/disk1s1                           500Mi   6.1Mi   483Mi     2%      34  4.9M    0%   /System/Volumes/iSCPreboot
        /dev/disk1s3                           500Mi   276Ki   483Mi     1%      36  4.9M    0%   /System/Volumes/Hardware
        /dev/disk3s5                           926Gi   416Gi   475Gi    47%    3.4M  5.0G    0%   /System/Volumes/Data
        map auto_home                            0Bi     0Bi     0Bi   100%       0     0     -   /System/Volumes/Data/home
        //st-build@st-nas.bside.com/ST_Share    12Ti    11Ti   1.3Ti    89%     11G  1.4G   89%   /Volumes/ST_Share
        //st-build@st-nas.bside.com/web         31Ti    24Ti   7.4Ti    77%     26G  8.0G   76%   /Volumes/web
        /dev/disk5s1                            16Gi    16Gi   467Mi    98%    507k  4.8M   10%   /Library/Developer/CoreSimulator/Volumes/iOS_21C62

        ➜  ~ df -h | grep -e Volumes/Data$
        /dev/disk3s5                           926Gi   416Gi   475Gi    47%    3.4M  5.0G    0%   /System/Volumes/Data
        */

        var sshResult = new List<DiskUsageInfo>[serverList.Count];
        foreach (var (server, index) in serverList.Select((e, i) => (e, i)))
        {
            var args = $"{server} \"df -h | grep -e /System/Volumes/Data$ -e /Volumes/ExternalDisk\"";
            Log.Debug($"ssh {args}");

            if (OutProcess.Run("ssh", args, waitMsec: 5000, out var result) == false)
            {
                Log.Error($"Failed to get disk usage info from {server}");
                continue;
            }

            sshResult[(int)index] = new List<DiskUsageInfo>();
            var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 9)
                {
                    Log.Error($"Invalid disk usage info: {line}");
                    continue;
                }

                // 1: 총 크기, 2: 사용량, 3: 남은용량, 4: 사용률, ... 8: 마운트 위치
                var diskType = tokens[8].Contains("System/Volumes/Data") ? "In." : "Ex.";
                var info = new DiskUsageInfo(server, diskType, tokens[1], tokens[2], tokens[3], tokens[4]);
                sshResult[(int)index].Add(info);
                Log.Debug($"{server} {diskType} - {info.UsageRate}");
            }
        }

        if (sshResult.Any(e => e == null))
        {
            output = null;
            return false;
        }

        output = [.. sshResult.SelectMany(e => e).OrderBy(e => e.Server)];
        return true;
    }

    private sealed record DiskUsageInfo(
        string Server,
        string DiskType,
        string TotalSize,
        string UsedSize,
        string AvailableSize,
        string UsageRate)
    {
        public string GetProgressBar()
        {
            int rate = int.Parse(this.UsageRate.TrimEnd('%'));
            int progressLength = this.TotalSize switch
            {
                "228Gi" => 5,
                "466Gi" => 10,
                "926Gi" => 20,
                "931Gi" => 20,
                "1.8Ti" => 40,
                _ => 0,
            };

            return ProgressBar.CreateSimple(rate, progressLength);
        }
    }
}
