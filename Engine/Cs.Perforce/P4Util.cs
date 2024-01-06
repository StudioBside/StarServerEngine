namespace Cs.Perforce;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Cs.Core.Util;

public static class P4Util
{
    private const string Separator = "@@";
 
    public static bool TryGetP4Workspace([MaybeNullWhen(false)] out string workspace)
    {
        workspace = null;
        var host = Dns.GetHostName();
        if (string.IsNullOrEmpty(host))
        {
            return false;
        }

        if (OutProcess.Run("p4", $"-F \"%client%{Separator}%Host%{Separator}%Root%\" -ztag clients", out string p4Output) == false)
        {
            return false;
        }

        //p4 워크스페이스 정보 가공
        var clientList = new Dictionary<string, List<P4ClientRoot>>();
        foreach (var line in p4Output.Split(Environment.NewLine))
        {
            var tokens = line.Split(Separator, 3, StringSplitOptions.None); // count=2이면 첫 번째 공백에서만 잘라줌. 빈 토큰은 알아서 정리.
            if (tokens.Length < 3)
            {
                continue;
            }

            string outputWorkspace = tokens[0];
            string outputHost = tokens[1].ToLower();
            string outputRoot = tokens[2].ToLower();

            if (clientList.TryGetValue(outputHost, out var outputList) == false)
            {
                outputList = new List<P4ClientRoot>();
                clientList.Add(outputHost, outputList);
            }

            outputList.Add(new P4ClientRoot(outputWorkspace, outputRoot));
        }

        //host 기반 스트림 검색
        string workingDirectory = System.IO.Directory.GetCurrentDirectory().ToLower();
        if (clientList.TryGetValue(host, out var list) == true)
        {
            foreach (var data in list)
            {
                if (workingDirectory.StartsWith(data.Root))
                {
                    workspace = data.Client;
                }
            }
        }

        //모든 데이터에서 스트림검색
        if (string.IsNullOrWhiteSpace(workspace))
        {
            foreach (var data in clientList.SelectMany(e => e.Value))
            {
                if (workingDirectory.StartsWith(data.Root))
                {
                    workspace = data.Client;
                }
            }
        }

        return string.IsNullOrWhiteSpace(workspace) == false;
    }

    public static Dictionary<string, string>? GetEnvironment()
    {
        if (OutProcess.Run("p4", "set", out string p4Output) == false)
        {
            return null;
        }

        var result = new Dictionary<string, string>();
        foreach (Match match in Regex.Matches(p4Output, @"^(P4.+)=(.+) \(set\)", RegexOptions.Multiline))
        {
            result.Add(match.Groups[1].Value, match.Groups[2].Value);
        }

        return result;
    }

    private sealed record P4ClientRoot(string Client, string Root);
}
