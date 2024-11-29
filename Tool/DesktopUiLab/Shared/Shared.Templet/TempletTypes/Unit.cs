namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Errors;
using Shared.Templet.Images;
using Shared.Templet.Strings;
using Shared.Templet.UnitScripts;
using static NKM.NKMOpenEnums;

public sealed class Unit : ITemplet, ISearchable
{
    // 동일한 이미지 파일 에러는 하나씩만 표시하기 위해 static으로 상태 관리
    private static readonly HashSet<string> MissingFiles = new();
    private static readonly string AnonymousImage;
    private static readonly string MissingImage;
    private readonly string nameStringId = string.Empty;
    private readonly string? scriptFileName;

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
        this.UnitFace = token.GetString("m_UnitFace");
        this.UnitType = token.GetEnum<NKM_UNIT_TYPE>("m_NKM_UNIT_TYPE");
        this.nameStringId = token.GetString("m_UnitNameString");
        this.IllustName = token.GetString("m_IllustName", null!);
        this.IllustNameUi = token.GetString("m_IllustNameUI", null!);
        this.scriptFileName = token.GetString("m_UnitTempletFileName", null!);
    }

    public static IEnumerable<Unit> Values => TempletContainer<Unit>.Values;
    public static string SmallImageRootPath { get; set; } = string.Empty;
    public static string ImageRootPath { get; set; } = string.Empty;
    int ITemplet.Key => this.Id;
    public int Id { get; }
    public string StrId { get; }
    public string Comment { get; }
    public string UnitFaceSmall { get; }
    public string UnitFace { get; }
    public string? IllustName { get; }
    public string? IllustNameUi { get; }
    public string SmallImageFullPath { get; private set; } = string.Empty;
    public string ImageFullPath { get; private set; } = string.Empty;
    public string IllustPath { get; private set; } = string.Empty;
    public string IllustUiPath { get; private set; } = string.Empty;
    public NKM_UNIT_TYPE UnitType { get; }
    public string Name { get; private set; } = string.Empty;
    public UnitScript? Script { get; private set; }
    public string DebugName => $"[{this.Id}] {this.Name}";

    public void Join()
    {
        if (string.IsNullOrEmpty(ImageRootPath))
        {
            Log.ErrorAndExit("Unit.ImageRootPath is not set.");
        }

        if (string.IsNullOrEmpty(SmallImageRootPath))
        {
            Log.ErrorAndExit("Unit.SmallImageRootPath is not set.");
        }

        this.Name = StringTable.Instance.Find(this.nameStringId);

        if (string.IsNullOrEmpty(this.UnitFaceSmall) != false)
        {
            this.SmallImageFullPath = AnonymousImage;
        }
        else
        {
            this.SmallImageFullPath = Path.Combine(SmallImageRootPath, this.UnitFaceSmall);
            this.SmallImageFullPath = Path.GetFullPath(this.SmallImageFullPath);

            if (File.Exists(this.SmallImageFullPath) == false)
            {
                this.SmallImageFullPath = MissingImage;

                if (MissingFiles.Add(this.UnitFaceSmall))
                {
                    ErrorMessage.Add(ErrorType.Unit, $"{this.DebugName} UnitFaceSmall 이미지가 존재하지 않습니다: {this.UnitFaceSmall}", this);
                }
            }
        }

        if (string.IsNullOrEmpty(this.UnitFace) != false)
        {
            this.ImageFullPath = AnonymousImage;
        }
        else
        {
            this.ImageFullPath = Path.Combine(ImageRootPath, this.UnitFace);
            this.ImageFullPath = Path.GetFullPath(this.ImageFullPath);

            if (File.Exists(this.ImageFullPath) == false)
            {
                this.ImageFullPath = MissingImage;

                ErrorMessage.Add(ErrorType.Unit, $"{this.DebugName} UnitFace 이미지가 존재하지 않습니다: {this.UnitFace}", this);
            }
        }

        if (this.IllustName is not null)
        {
            if (PathResolver.Instance.TryGetIllustPath(this.IllustName, out var path))
            {
                this.IllustPath = path;
            }
            else if (MissingFiles.Add(this.IllustName))
            {
                ErrorMessage.Add(ErrorType.Unit, $"{this.DebugName} Illust 이미지가 존재하지 않습니다: {this.IllustName}", this);
            }
        }

        if (this.IllustNameUi is not null)
        {
            if (PathResolver.Instance.TryGetIllustPath(this.IllustNameUi, out var path))
            {
                this.IllustUiPath = path;
            }
            else if (MissingFiles.Add(this.IllustNameUi))
            {
                ErrorMessage.Add(ErrorType.Unit, $"{this.DebugName} Illust UI 이미지가 존재하지 않습니다: {this.IllustNameUi}", this);
            }
        }

        if (string.IsNullOrEmpty(this.scriptFileName) == false)
        {
            this.Script = UnitScriptContainer.Instance.Find(this.scriptFileName);
            if (this.Script is null)
            {
                ErrorMessage.Add(ErrorType.Unit, $"{this.DebugName} 스크립트 파일을 찾을 수 없습니다: {this.scriptFileName}", this);
            }
            else
            {
                this.Script.AddReference(this);
            }
        }
    }

    public void Validate()
    {
        if (this.nameStringId == this.Name)
        {
            ErrorMessage.Add(ErrorType.Unit, $"유닛 {this.DebugName} 의 이름 스트링을 찾을 수 없습니다.", this);
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
