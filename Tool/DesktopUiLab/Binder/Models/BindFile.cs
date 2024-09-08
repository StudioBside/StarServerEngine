namespace Binder.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class BindFile
{
    private readonly string filePath;
    public BindFile(string filePath)
    {
        this.filePath = filePath;
        this.Name = Path.GetFileNameWithoutExtension(filePath);
        this.LastWriteTime = File.GetLastWriteTime(filePath);
    }

    public string Name { get; }
    public DateTime LastWriteTime { get; }
}
