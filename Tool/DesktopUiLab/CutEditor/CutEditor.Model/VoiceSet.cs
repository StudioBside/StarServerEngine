namespace CutEditor.Model;

using System;
using System.Text;
using Cs.Core.Util;
using Shared.Interfaces;
using static StringStorage.Enums;

public sealed class VoiceSet : ISearchable
{
    private readonly VoiceFile[] l10nData = new VoiceFile[EnumUtil<L10nType>.Count];

    public VoiceSet(string fileName, string displayPath)
    {
        this.DisplayPath = displayPath;
        this.FileNameOnly = Path.GetFileName(fileName);
    }

    public string DisplayPath { get; }
    public string FileNameOnly { get; }

    public VoiceFile Korean => this.GetData(L10nType.Korean);
    public VoiceFile Japanese => this.GetData(L10nType.Japanese);
    public VoiceFile ChineseSimplified => this.GetData(L10nType.ChineseSimplified);
    public VoiceFile English => this.GetData(L10nType.English);

    public string GetDebugStatus()
    {
        const string notExists = "X";
        const string exists = "O";

        StringBuilder builder = new();
        builder.Append($"[Kr: {(this.Korean == null ? notExists : exists)}]  ");
        builder.Append($"[Jp: {(this.Japanese == null ? notExists : exists)}]  ");
        builder.Append($"[En: {(this.English == null ? notExists : exists)}]  ");
        builder.Append($"[Cn: {(this.ChineseSimplified == null ? notExists : exists)}]");

        return builder.ToString();
    }

    public VoiceFile GetData(L10nType type)
    {
        return this.l10nData[(int)type];
    }

    public void SetData(L10nType type, VoiceFile voiceFile)
    {
        this.l10nData[(int)type] = voiceFile;
    }

    public bool IsTarget(string keyword) => this.FileNameOnly.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}
