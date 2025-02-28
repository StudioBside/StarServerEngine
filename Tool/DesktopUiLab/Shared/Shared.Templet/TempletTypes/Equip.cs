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

public sealed class Equip : ITemplet, ISearchable
{
    private readonly string nameStringId = string.Empty;

    public Equip(JToken token)
    {
        this.Id = token.GetInt32("EquipItemId");
        this.nameStringId = token.GetString("EquipItemName");
    }

    public static IEnumerable<Equip> Values => TempletContainer<Equip>.Values;
    public int Id { get; }
    int ITemplet.Key => this.Id;
    public StringElement? NameElement { get; private set; }
    public string Name => this.NameElement?.Korean ?? string.Empty;
    public string DebugName => $"[{this.Id}] {this.Name}";

    public void Join()
    {
        this.NameElement = StringTable.Instance.FindElement(this.nameStringId);
    }

    public void Validate()
    {
        if (this.nameStringId == this.Name)
        {
            ErrorMessage.Add(ErrorType.Unit, $"아르카나 {this.DebugName} 의 이름 스트링을 찾을 수 없습니다.", this);
        }
    }

    bool ISearchable.IsTarget(string keyword)
    {
        return this.Id.ToString().Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
               this.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase);
    }
}
