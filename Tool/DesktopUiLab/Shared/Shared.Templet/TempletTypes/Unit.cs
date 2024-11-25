namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Strings;
using static NKM.NKMOpenEnums;

public sealed class Unit : ITemplet, ISearchable
{
    private string nameStringId = string.Empty;

    public Unit(JToken token)
    {
        this.Id = token.GetInt32("m_UnitID");
        this.StrId = token.GetString("m_UnitStrID");
        this.Comment = token.GetString("Comment");
        this.ImageFileName = token.GetString("m_UnitFaceSmall");
        this.UnitType = token.GetEnum<NKM_UNIT_TYPE>("m_NKM_UNIT_TYPE");
        this.nameStringId = token.GetString("m_UnitNameString");
    }

    public static string ImageRootPath { get; set; } = string.Empty;
    int ITemplet.Key => this.Id;
    public int Id { get; }
    public string StrId { get; }
    public string Comment { get; }
    public string ImageFileName { get; }
    public string ImageFullPath { get; private set; } = string.Empty;
    public NKM_UNIT_TYPE UnitType { get; }
    public string Name { get; private set; } = string.Empty;
    public string DebugName => $"[{this.Id}]{this.Name}";

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

        this.Name = StringTable.Instance.Find(this.nameStringId);
    }

    public void Validate()
    {
    }

    bool ISearchable.IsTarget(string keyword)
    {
        return this.Id.ToString().Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
               this.StrId.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
               this.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
               this.Comment.Contains(keyword, StringComparison.CurrentCultureIgnoreCase);
    }

    public bool EnableForCutscene()
    {
        return this.UnitType == NKM_UNIT_TYPE.NUT_SAVIOR ||
            this.UnitType == NKM_UNIT_TYPE.NUT_NPC;
    }

    public override string ToString()
    {
        return this.DebugName;
    }
}
