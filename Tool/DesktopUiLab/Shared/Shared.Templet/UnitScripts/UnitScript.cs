namespace Shared.Templet.UnitScripts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class UnitScript
{
    public UnitScript(string path)
    {
        this.FullPath = Path.GetFullPath(path);
        this.FileName = Path.GetFileName(path);
    }

    public string FileName { get; set; }
    public string FullPath { get; set; }
}
