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

internal sealed class CpuRamInfo : StringContainsReaction
{
    public override string Trigger => "[빌드머신 스펙]";
    private string DebugName => $"[CpuRamInfo]";

    public override Block GetIntroduceBlock()
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            writer.WriteLine($"트리거 : `{this.Trigger}`");
            writer.WriteLine($"효과 : 클라이언트 빌드 머신의 cpu / ram 정보를 표시합니다.");
        }

        return new SectionBlock { Text = new Markdown(sb.ToString()) };
    }

    public override async Task Process(ISlackApiClient slack, TargetEventRecord eventRecord)
    {
        var targetEvent = eventRecord.TargetEvent;
        string userName = await targetEvent.GetUserNameAsync(slack);
        string channelName = await eventRecord.OriginalEvent.GetChannelNameAsync(slack);

        Log.Debug($"{this.DebugName} message:{targetEvent.Text} userName:{userName} channelName:{channelName}");

        if (GetHwSpecInfo(out var usageInfos) == false)
        {
            await slack.Chat.PostMessage(new Message
            {
                Text = "원격 장비 정보 조회에 실패했습니다.",
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

    private static Message ConvertToSlackMessage(IReadOnlyList<HwSpecInfo> specInfos)
    {
        var message = new Message
        {
            IconEmoji = ":apple-logo:",
            Username = "Tim Cook",
        };

        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);

        foreach (var specInfo in specInfos)
        {
            var serverName = specInfo.Server
                .Split('@').Last() // 앞부분에 계정 정보 잘라낸다.
                .Split('.').First(); // 뒷부분에 도메인 정보 잘라낸다.

            writer.WriteLine($"*{serverName}* - {specInfo.CpuBrand} / {specInfo.Memory}");
        }

        message.Text = sb.ToString();
        return message;
    }

    private static bool GetHwSpecInfo([MaybeNullWhen(false)] out IReadOnlyList<HwSpecInfo> output)
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
        ➜ sysctl -n machdep.cpu.brand_string
        Apple M4 Pro
        ➜  ~ system_profiler SPHardwareDataType | grep "Memory:"
            Memory: 64 GB
        */

        var sshResult = new HwSpecInfo[serverList.Count];
        foreach (var (server, index) in serverList.Select((e, i) => (e, i)))
        {
            var args = $"{server} \"sysctl -n machdep.cpu.brand_string;system_profiler SPHardwareDataType | grep 'Memory:'\"";
            Log.Debug($"ssh {args}");

            if (OutProcess.Run("ssh", args, waitMsec: 5000, out var result) == false)
            {
                Log.Error($"Failed to get disk usage info from {server}");
                continue;
            }

            var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 2)
            {
                Log.Error($"Failed to get disk usage info from {server}");
                continue;
            }

            var cpuBrand = lines[0];
            var memory = lines[1].Trim().Replace("Memory: ", string.Empty);

            var info = new HwSpecInfo(server, cpuBrand, memory);
            sshResult[index] = info;
            Log.Debug($"{server} {cpuBrand} {memory}");
        }

        if (sshResult.Any(e => e == null))
        {
            output = null;
            return false;
        }

        output = sshResult;
        return true;
    }

    private sealed record HwSpecInfo(string Server, string CpuBrand, string Memory);
}
