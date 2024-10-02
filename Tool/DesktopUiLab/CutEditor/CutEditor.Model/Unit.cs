namespace CutEditor.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public sealed class Unit
{
    public Unit(JToken token)
    {
    }

    public int Id { get; }
}
