namespace CutEditor.Model;
using System;
using System.Collections.Generic;
using static StringStorage.Enums;

public sealed class VoiceFile
{
    private static readonly Dictionary<string, L10nType> L10nPathes = new()
            {
                { "KOR", L10nType.Korean },
                { "JPN", L10nType.Japanese },
                { "CHN", L10nType.ChineseSimplified },
                { "ENG", L10nType.English },
            };

    private VoiceFile(string fileName, string displayPath)
    {
        this.FileName = fileName;
        this.DisplayPath = displayPath;
        this.FullPath = Path.GetFullPath(fileName);
        this.FileNameOnly = Path.GetFileName(fileName);
    }

    public string FileName { get; }
    public string FullPath { get; }
    public string DisplayPath { get; }
    public string FileNameOnly { get; }
    public L10nType Language { get; }

    public static bool TryCreate(string fileName, out VoiceFile? result, out L10nType? language)
    {
        result = null;
        language = null;

        var pathSplits = fileName.Split(Path.DirectorySeparatorChar);
        for (int i = 0; i < pathSplits.Length; i++)
        {
            if (L10nPathes.TryGetValue(pathSplits[i], out var value))
            {
                int targetIndex = i + 1;
                string path = string.Join(Path.DirectorySeparatorChar.ToString(), pathSplits, targetIndex, pathSplits.Length - targetIndex);
                result = new VoiceFile(fileName, path);
                language = value;

                return true;
            }
        }

        return false;
    }
}