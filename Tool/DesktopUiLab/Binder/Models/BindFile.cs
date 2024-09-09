namespace Binder.Models;

using System;
using System.IO;

public sealed class BindFile
{
    private readonly string filePath;
    private readonly List<Extract> extracts = new();

    public BindFile(string filePath)
    {
        this.filePath = filePath;
        this.Name = Path.GetFileNameWithoutExtension(filePath);
        this.LastWriteTime = File.GetLastWriteTime(filePath);
    }

    public string Name { get; }
    public DateTime LastWriteTime { get; }
    public IList<Extract> Extracts => this.extracts;
}