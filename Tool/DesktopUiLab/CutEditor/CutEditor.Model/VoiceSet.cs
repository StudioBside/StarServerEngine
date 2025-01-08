namespace CutEditor.Model;

using System;
using Cs.Core.Util;
using static StringStorage.Enums;

internal sealed class VoiceSet
{
    private readonly bool[] hasData = new bool[EnumUtil<L10nType>.Count];

    public VoiceSet(string fileName, string unitName)
    {
        this.FileName = fileName;
        this.UnitName = unitName;
    }

    public string FileName { get; }
    public string UnitName { get; }

    public bool HasData(L10nType type)
    {
        return this.hasData[(int)type];
    }

    public void SetDataExists(L10nType type)
    {
        this.hasData[(int)type] = true;
    }
}
