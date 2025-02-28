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

public sealed class Arcana : ITemplet, ISearchable
{
    private readonly string nameStringId = string.Empty;
    private readonly string titleStringId = string.Empty;

    public Arcana(JToken token)
    {
        this.Id = token.GetInt32("ArcanaId");
        this.nameStringId = token.GetString("ArcanaName");
        this.titleStringId = token.GetString("ArcanaTitle");
    }

    public static IEnumerable<Arcana> Values => TempletContainer<Arcana>.Values;
    public static string ImageRootPath { get; set; } = string.Empty;
    public int Id { get; }
    int ITemplet.Key => this.Id;
    public StringElement? NameElement { get; private set; }
    public StringElement? TitleElement { get; private set; }
    public string Name => this.NameElement?.Korean ?? string.Empty;
    public string Title => this.TitleElement?.Korean ?? string.Empty;
    public string DebugName => $"[{this.Id}] {this.Title} {this.Name}";

    public void Join()
    {
        this.NameElement = StringTable.Instance.FindElement(this.nameStringId);
        this.TitleElement = StringTable.Instance.FindElement(this.titleStringId);
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
