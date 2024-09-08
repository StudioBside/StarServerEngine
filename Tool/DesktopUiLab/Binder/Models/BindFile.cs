namespace Binder.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class BindFile
{
    public BindFile(string name)
    {
        this.Name = name;
    }

    public string Name { get; }
}
