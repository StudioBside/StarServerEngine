namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Errors;
using Shared.Templet.Strings;
using static NKM.NKMOpenEnums;

public sealed class Unit : ITemplet, ISearchable
{
    private static readonly HashSet<string> MissingFaceSmall = new();
    private static readonly string AnonymousImage;
    private static readonly string MissingImage;
    private readonly string nameStringId = string.Empty;

    static Unit()
    {
        AnonymousImage = Path.GetFullPath("./Assets/Images/anonymous.png");
        MissingImage = Path.GetFullPath("./Assets/Images/missing.png");
    }

    public Unit(JToken token)
    {
        this.Id = token.GetInt32("m_UnitID");
        this.StrId = token.GetString("m_UnitStrID");
        this.Comment = token.GetString("Comment");
        this.UnitFaceSmall = token.GetString("m_UnitFaceSmall");
        this.UnitType = token.GetEnum<NKM_UNIT_TYPE>("m_NKM_UNIT_TYPE");
        this.nameStringId = token.GetString("m_UnitNameString");
    }

    public static string ImageRootPath { get; set; } = string.Empty;
    int ITemplet.Key => this.Id;
    public int Id { get; }
    public string StrId { get; }
    public string Comment { get; }
    public string UnitFaceSmall { get; }
    public string ImageFullPath { get; private set; } = string.Empty;
    public NKM_UNIT_TYPE UnitType { get; }
    public string Name { get; private set; } = string.Empty;
    public string DebugName => $"[{this.Id}] {this.Name}";

    public void Join()
    {
        if (string.IsNullOrEmpty(ImageRootPath))
        {
            Log.ErrorAndExit("Unit.ImageRootPath is not set.");
        }

        this.Name = StringTable.Instance.Find(this.nameStringId);

        if (string.IsNullOrEmpty(this.UnitFaceSmall) != false)
        {
            this.ImageFullPath = AnonymousImage;
        }
        else
        {
            this.ImageFullPath = Path.Combine(ImageRootPath, this.UnitFaceSmall);
            this.ImageFullPath = Path.GetFullPath(this.ImageFullPath);

            if (File.Exists(this.ImageFullPath) == false)
            {
                this.ImageFullPath = MissingImage;

                if (MissingFaceSmall.Add(this.UnitFaceSmall))
                {
                    ErrorMessageController.Instance.Add(
                        ErrorType.Unit,
                        $"{this.DebugName} 이미지가 존재하지 않습니다. UnitFaceSmall:{this.UnitFaceSmall}");
                }
            }
        }
    }

    public void Validate()
    {
        if (this.nameStringId == this.Name)
        {
            ErrorMessageController.Instance.Add(
                ErrorType.String,
                $"유닛 {this.DebugName} 의 이름 스트링을 찾을 수 없습니다. nameStringId:{this.nameStringId}");
        }
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
