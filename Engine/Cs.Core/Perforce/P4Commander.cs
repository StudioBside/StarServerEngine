namespace Cs.Core.Perforce;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Cs.Core.Util;
using Cs.Logging;

public readonly record struct P4Commander
{
    public static readonly string CommandName = DevOps.IsRunningInLinux() ? "p4.exe" : "p4";

    private P4Commander(string stream, string clientRoot)
    {
        this.Stream = stream;
        this.ClientRoot = clientRoot;
    }

    public string ClientRoot { get; init; }
    public string Stream { get; init; }

    public static bool TryCreate(out P4Commander result)
    {
        var p4ClientInfo = DevOps.GetP4ClientInfoForDev();
        if (p4ClientInfo is null)
        {
            Log.Error($"Failed to get p4 workspace info.");
            result = default;
            return false;
        }

        var streamHead = $"//stream/{p4ClientInfo.StreamInfo.NameSensitive}";
        result = new P4Commander(streamHead, p4ClientInfo.Root);
        return true;
    }

    public bool OpenForEdit(string localFilePath, out string p4Output)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);

        if (OutProcess.Run(CommandName, $"edit {depotPath}", out p4Output) == false)
        {
            return false;
        }

        return true;
    }

    public bool OpenForEdit(string localPath, string extension, out string p4Output)
    {
        var depotPath = this.ToDepotPath(localPath, extension);

        if (OutProcess.Run(CommandName, $"edit {depotPath}", out p4Output) == false)
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

        if (OutProcess.Run(CommandName, $"delete {depotPath}", out p4Output) == false)
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

        if (OutProcess.Run(CommandName, $"revert -a {depotPath}", out p4Output) == false)
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

        if (OutProcess.Run(CommandName, $"revert {depotPath}", out p4Output) == false)
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
        OutProcess.Run(CommandName, $"reconcile -a {depotPath}", out var p4Output);
    }

    public void AddNewFile(string localFilePath)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);

        // note: 더할 파일이 없으면 실패처리된다.
        OutProcess.Run(CommandName, $"reconcile -a {depotPath}", out var p4Output);
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

        if (OutProcess.Run(CommandName, $"diff -se {depotPath}", out var p4Output) == false)
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

        OutProcess.Run(CommandName, $"opened {depotPath}", out var p4Output);
        return p4Output.Contains(" - edit") || // 이미 열려있을 때
            p4Output.Contains(" - add "); // 신규 생성된 파일
    }

    public bool CheckIfRegistered(string localFilePath)
    {
        var depotPath = this.ToSingleDepotPath(localFilePath);
        OutProcess.Run(CommandName, $"fstat {depotPath}", out var p4Output);
        return p4Output.Contains("headRev");
    }

    public List<string>? GetOpenedFiles(string localPath, string extension, out string p4Output)
    {
        var depotPath = this.ToDepotPath(localPath, extension);

        if (OutProcess.Run(CommandName, $"opened {depotPath}", out p4Output) == false)
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
