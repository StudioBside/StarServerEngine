namespace CutEditor.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;

public sealed class Unit
{
    public Unit(JToken token)
    {
        this.Id = token.GetInt32("m_UnitID");
        this.Comment = token.GetString("Comment");
        this.ImageFileName = token.GetString("m_UnitFaceSmall");
    }

    public int Id { get; }
    public string Comment { get; }
    public string ImageFileName { get; }
}
