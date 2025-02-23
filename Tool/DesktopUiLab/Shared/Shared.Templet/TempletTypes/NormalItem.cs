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

public sealed class NormalItem : ITemplet, ISearchable
{
    private static readonly HashSet<string> MissingFiles = new();
    private static readonly string AnonymousImage;
    private static readonly string MissingImage;

    private readonly string nameStringId = string.Empty;
    private readonly string titleStringId = string.Empty;

    static NormalItem()
    {
        AnonymousImage = Path.GetFullPath("./Assets/Images/anonymous.png");
        MissingImage = Path.GetFullPath("./Assets/Images/missing.png");
    }

    public NormalItem(JToken token)
    {
        this.Id = token.GetInt32("ArcanaId");
        this.nameStringId = token.GetString("ArcanaName");
        this.titleStringId = token.GetString("ArcanaTitle");
        this.ArcanaEpisode = token.GetString("ArcanaEpisode");
        this.UnitFace = token.GetString("UnitFace");
    }

    public static IEnumerable<Equip> Values => TempletContainer<Equip>.Values;
    public static string ImageRootPath { get; set; } = string.Empty;
    public int Id { get; }
    int ITemplet.Key => this.Id;
    public StringElement? NameElement { get; private set; }
    public StringElement? TitleElement { get; private set; }
    public string ArcanaEpisode { get; }
    public string UnitFace { get; }
    public string ImageFullPath { get; private set; } = string.Empty;
    public string Name => this.NameElement?.Korean ?? string.Empty;
    public string Title => this.TitleElement?.Korean ?? string.Empty;
    public string DebugName => $"[{this.Id}] {this.Title} {this.Name}";

    public void Join()
    {
        if (string.IsNullOrEmpty(ImageRootPath))
        {
            Log.ErrorAndExit("Arcana.ImageRootPath is not set.");
        }

        this.NameElement = StringTable.Instance.FindElement(this.nameStringId);
        this.TitleElement = StringTable.Instance.FindElement(this.titleStringId);

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

                if (MissingFiles.Add(this.UnitFace))
                {
                    ErrorMessage.Add(ErrorType.Unit, $"{this.DebugName} UnitFace 이미지가 존재하지 않습니다: {this.UnitFace}", this);
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
    }

    public void Validate()
    {
        if (this.nameStringId == this.Name)
        {
            ErrorMessage.Add(ErrorType.Unit, $"아르카나 {this.DebugName} 의 이름 스트링을 찾을 수 없습니다.", this);
        }

        if (this.titleStringId == this.Title)
        {
            ErrorMessage.Add(ErrorType.Unit, $"아르카나 {this.DebugName} 의 타이틀 스트링을 찾을 수 없습니다.", this);
        }
    }

    bool ISearchable.IsTarget(string keyword)
    {
        return this.Id.ToString().Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
               this.Title.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
               this.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase);
    }
}
