namespace Shared.Templet.TempletTypes;

using System;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Errors;
using Shared.Templet.Images;
using Shared.Templet.Strings;

public sealed class SkillTemplet : ITemplet, ISearchable
{
    private readonly List<Unit> references = new();
  
    private readonly string nameStringId;
    private readonly string descStringId;
    private string skillIcon;

    public SkillTemplet(JToken token)
    {
        this.Id = token.GetInt32("m_SkillID");
        this.StrId = token.GetString("m_SkillStrID");
        this.nameStringId = token.GetString("m_SkillName");
        this.descStringId = token.GetString("m_SkillDesc");
        this.skillIcon = token.GetString("m_SkillIcon");
    }

    public static IEnumerable<SkillTemplet> Values => TempletContainer<SkillTemplet>.Values;
    public int Key => this.Id;
    public int Id { get; }
    public string StrId { get; }
    public StringElement? NameElement { get; private set; }
    public StringElement? DescElement { get; private set; }
    public string Name => this.NameElement?.Korean ?? string.Empty;
    public string Desc => this.DescElement?.Korean ?? string.Empty;
    public string IconPath { get; private set; } = string.Empty;
    public string DebugName => $"[{this.Id}] {this.Name}";
    public string DebugId => $"[{this.Id}] {this.StrId}";
    public IReadOnlyList<Unit> References => this.references;
    public Unit? FirstReference => this.references.Count > 0 ? this.references[0] : null;

    public void Join()
    {
        this.NameElement = StringTable.Instance.FindElement(this.nameStringId);
        this.DescElement = StringTable.Instance.FindElement(this.descStringId);

        if (PathResolver.Instance.TryGetSkillPath(this.skillIcon, out var path))
        {
            this.IconPath = path;
        }
        else
        {
            ErrorMessage.Add(ErrorType.Skill, $"{this.DebugName} 의 아이콘 파일을 찾을 수 없습니다. iconName:{this.skillIcon}", this);
        }
    }

    public void Validate()
    {
        if (this.nameStringId == this.Name)
        {
            ErrorMessage.Add(ErrorType.Skill, $"{this.DebugName} 의 이름 스트링을 찾을 수 없습니다.", this);
        }

        if (this.descStringId == this.Desc)
        {
            ErrorMessage.Add(ErrorType.Skill, $"{this.DebugName} 의 설명 스트링을 찾을 수 없습니다.", this);
        }
    }

    bool ISearchable.IsTarget(string keyword)
    {
        return this.Key.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               this.StrId.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               this.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               this.Desc.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    internal void AddReference(Unit unit)
    {
        this.references.Add(unit);
    }
}
