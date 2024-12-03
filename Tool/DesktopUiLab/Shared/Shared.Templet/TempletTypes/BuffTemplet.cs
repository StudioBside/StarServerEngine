namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Errors;
using Shared.Templet.Images;
using Shared.Templet.Strings;

public sealed class BuffTemplet : ITemplet, ISearchable
{
    private string nameId;
    private string descId;
    private string iconName;

    public BuffTemplet(JToken token)
    {
        this.Id = token.GetInt32("m_BuffID");
        this.StrId = token.GetString("m_BuffStrID");
        this.nameId = token.GetString("m_BuffName");
        this.descId = token.GetString("m_BuffDesc");
        this.iconName = token.GetString("m_IconName");
    }

    public static IEnumerable<BuffTemplet> Values => TempletContainer<BuffTemplet>.Values;
    int ITemplet.Key => this.Id;
    public int Id { get; }
    public string StrId { get; }
    public string Name { get; private set; } = string.Empty;
    public string Desc { get; private set; } = string.Empty;
    public string IconPath { get; private set; } = string.Empty;
    public string DebugName => $"[{this.Id} {this.Name}]";

    bool ISearchable.IsTarget(string keyword)
    {
        return this.Id.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               this.StrId.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               this.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               this.Desc.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    public void Join()
    {
        this.Name = StringTable.Instance.Find(this.nameId);
        this.Desc = StringTable.Instance.Find(this.descId);
        if (this.Name == this.nameId)
        {
            ErrorMessage.Add(ErrorType.Buff, $"{this.DebugName} 이름 스트링이 지정되지 않았습니다. nameId:{this.nameId}");
        }

        if (this.Desc == this.descId)
        {
            ErrorMessage.Add(ErrorType.Buff, $"{this.DebugName} 설명 스트링이 지정되지 않았습니다. descId:{this.descId}");
        }

        if (PathResolver.Instance.TryGetBuffPath(this.iconName, out var path))
        {
            this.IconPath = path;
        }
        else
        {
            ErrorMessage.Add(ErrorType.Buff, $"{this.DebugName} 아이콘 파일이 존재하지 않습니다. iconName:{this.iconName}");
        }
    }

    public void Validate()
    {
    }
}
