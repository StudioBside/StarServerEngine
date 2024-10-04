namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Templet.Base;

public sealed class Unit : ITemplet
{
    public Unit(JToken token)
    {
        this.Id = token.GetInt32("m_UnitID");
        this.StrId = token.GetString("m_UnitStrID");
        this.Comment = token.GetString("Comment");
        this.ImageFileName = token.GetString("m_UnitFaceSmall");
    }

    public static string ImageRootPath { get; set; } = string.Empty;
    int ITemplet.Key => this.Id;
    public int Id { get; }
    public string StrId { get; }
    public string Comment { get; }
    public string ImageFileName { get; }
    public string ImageFullPath { get; private set; } = string.Empty;

    public void Join()
    {
        if (string.IsNullOrEmpty(ImageRootPath))
        {
            Log.ErrorAndExit("Unit.ImageRootPath is not set.");
        }

        if (string.IsNullOrEmpty(this.ImageFileName) == false)
        {
            this.ImageFullPath = Path.Combine(ImageRootPath, this.ImageFileName);
            this.ImageFullPath = Path.GetFullPath(this.ImageFullPath);
        }
    }

    public void Validate()
    {
    }
}
