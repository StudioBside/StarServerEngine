namespace Cs.Core.Perforce;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Cs.Core.Util;
using Cs.Logging;

public readonly record struct P4Commander
{
    private const string Seperator = "@@";

    private P4Commander(string stream, string clientRoot)
    {
        this.Stream = stream;
        this.ClientRoot = clientRoot;
    }

    public string ClientRoot { get; init; }
    public string Stream { get; init; }

    public static bool TryCreate(out P4Commander result)
    {
        // 현재 위치를 기반으로 workspace 이름을 알아낸다.
        if (P4Util.TryGetP4Workspace(out var workspace) == false)
        {
            Log.Error($"Failed to get p4 workspace info.");
            result = default;
            return false;
        }

        // 알아낸 workspace 이름으로 stream, clientRoot를 알아낸다.
        if (OutProcess.Run("p4", $"-F \"%Stream%{Seperator}%Root%\" -ztag clients -E {workspace}", out string p4Output) == false)
        {
            Log.Error($"Failed to get p4 stream|root info. output:{p4Output}");
            result = default;
            return false;
        }

        //p4 워크스페이스 정보 가공
        foreach (var line in p4Output.Split(Environment.NewLine))
        {
            var tokens = line.Split(Seperator); // count=2이면 첫 번째 공백에서만 잘라줌. 빈 토큰은 알아서 정리.
            if (tokens.Length < 2)
            {
                continue;
            }

            string stream = tokens[0];
            string clientRoot = tokens[1];

            // 예외처리. dev의 virtual stream들은 dev로 변경해주어야 함. 
            if (stream.StartsWith("//stream/dev"))
            {
                stream = stream[..12];
            }

            result = new P4Commander(stream, clientRoot);
            return true;
        }

        Log.Error($"failed to parse p4 stream|root info. output:{p4Output}");
        result = default;
        return false;
    }

    public bool OpenForEdit(string localFilePath, out string p4Output)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);

        if (OutProcess.Run("p4", $"edit {depotPath}", out p4Output) == false)
        {
            return false;
        }

        return true;
    }

    public bool OpenForEdit(string localPath, string extension, out string p4Output)
    {
        var depotPath = this.ToDepotPath(localPath, extension);

        if (OutProcess.Run("p4", $"edit {depotPath}", out p4Output) == false)
        {
            return false;
        }

        return true;
    }

    public bool OpenForEdit(IEnumerable<string> localPathList, string extension, out string p4Output)
    {
        foreach (var localPath in localPathList)
        {
            if (this.OpenForEdit(localPath, extension, out p4Output) == false)
            {
                return false;
            }
        }

        p4Output = String.Empty;
        return true;
    }

    public bool Delete(string localFilePath, out string p4Output)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);

        if (OutProcess.Run("p4", $"delete {depotPath}", out p4Output) == false)
        {
            return false;
        }

        return true;
    }

    public bool RevertUnchnaged(string localPath, out string p4Output)
    {
        return this.RevertUnchnaged(localPath, extension: string.Empty, out p4Output);
    }

    public bool RevertUnchnaged(string localPath, string extension, out string p4Output)
    {
        var depotPath = this.ToDepotPath(localPath, extension);

        if (OutProcess.Run("p4", $"revert -a {depotPath}", out p4Output) == false)
        {
            return false;
        }

        return true;
    }

    public bool RevertUnchnaged(IEnumerable<string> localPathList, string extension, out string p4Output)
    {
        foreach (var localPath in localPathList)
        {
            if (this.RevertUnchnaged(localPath, extension, out p4Output) == false)
            {
                return false;
            }
        }

        p4Output = String.Empty;
        return true;
    }

    public bool RevertAll(string localPath, string extension, out string p4Output)
    {
        var depotPath = this.ToDepotPath(localPath, extension);

        if (OutProcess.Run("p4", $"revert {depotPath}", out p4Output) == false)
        {
            return false;
        }

        return true;
    }

    public bool RevertAll(IEnumerable<string> localPathList, string extension, out string p4Output)
    {
        foreach (var localPath in localPathList)
        {
            if (this.RevertAll(localPath, extension, out p4Output) == false)
            {
                return false;
            }
        }

        p4Output = String.Empty;
        return true;
    }
    
    public void AddNewFiles(string localDirPath, string extension)
    {
        var depotPath = this.ToDepotPath(localDirPath, extension);

        // note: 더할 파일이 없으면 실패처리된다.
        OutProcess.Run("p4", $"reconcile -a {depotPath}", out var p4Output);
    }

    public void AddNewFile(string localFilePath)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);

        // note: 더할 파일이 없으면 실패처리된다.
        OutProcess.Run("p4", $"reconcile -a {depotPath}", out var p4Output);
    }

    public void AddNewFiles(IEnumerable<string> localPathList, string extension)
    {
        foreach (var localPath in localPathList)
        {
            this.AddNewFiles(localPath, extension);
        }
    }

    public bool CheckIfChanged(string localFilePath, out bool result)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);

        if (OutProcess.Run("p4", $"diff -se {depotPath}", out var p4Output) == false)
        {
            result = false;
            return p4Output.Contains(" - file(s) up-to-date.") // 파일 변경내용이 없거나, 이미 열려있을 때
                || p4Output.Contains(" - file(s) not on client.") // 해당 폴더에 검색 조건인 파일이 없을 때
                ;
        }

        result = string.IsNullOrEmpty(p4Output) == false;
        return true;
    }

    public bool CheckIfOpened(string localFilePath)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);

        OutProcess.Run("p4", $"opened {depotPath}", out var p4Output);
        return p4Output.Contains(" - edit") || // 이미 열려있을 때
            p4Output.Contains(" - add "); // 신규 생성된 파일
    }

    public bool CheckIfRegistered(string localFilePath)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);
        OutProcess.Run("p4", $"fstat {depotPath}", out var p4Output);
        return p4Output.Contains("headRev");
    }

    public List<string>? GetOpenedFiles(string localPath, string extension, out string p4Output)
    {
        var depotPath = this.ToDepotPath(localPath, extension);

        if (OutProcess.Run("p4", $"opened {depotPath}", out p4Output) == false)
        {
            if (p4Output.Contains(" - file(s) not opened on this client."))
            {
                return new List<string>();
            }

            return null;
        }

        var result = new List<string>();
        foreach (Match match in Regex.Matches(p4Output, @"^(.+)#\d+ - edit .+", RegexOptions.Multiline))
        {
            var localOpenedPath = this.ToLocalPath(match.Groups[1].Value);
            result.Add(localOpenedPath);
        }

        return result;
    }

    //// --------------------------------------------------------------------------------------------------------

    private string ToSingleDepotPath(string localFilePath)
    {
        return Path.GetFullPath(localFilePath)
            .Replace(this.ClientRoot, this.Stream)
            .Replace('\\', '/');
    }

    private string ToDepotPath(string localPath, string extension)
    {
        var temp = Path.GetFullPath(localPath)
            .Replace(this.ClientRoot, this.Stream);

        return Path.Combine(temp, $"...{extension}")
            .Replace('\\', '/');
    }

    private string ToLocalPath(string depotPath)
    {
        return depotPath
            .Replace(this.Stream, this.ClientRoot)
            .Replace('/', '\\');
    }
}
