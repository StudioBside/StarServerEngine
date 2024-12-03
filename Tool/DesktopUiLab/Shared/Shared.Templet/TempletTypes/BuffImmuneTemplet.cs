namespace Shared.Templet.TempletTypes;

using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Templet.Base;
using Shared.Templet.Errors;

public sealed class BuffImmuneTemplet : IGroupTemplet
{
    private readonly HashSet<string> buffIds = new();
    private readonly List<BuffTemplet> buffTemplets = new();

    public BuffImmuneTemplet()
    {
    }

    public static IEnumerable<BuffImmuneTemplet> Values => TempletContainer<BuffImmuneTemplet>.Values;
    int ITemplet.Key => this.GroupId;
    public int GroupId { get; private set; }
    public string DebugName => $"[BuffImmuneTemplet. Id:{this.GroupId}]";
    public IReadOnlyList<BuffTemplet> BuffTemplets => this.buffTemplets;

    public void Join()
    {
        foreach (var buffId in this.buffIds)
        {
            if (TempletContainer<BuffTemplet>.TryGet(buffId, out var buffTemplet) == false)
            {
                ErrorMessage.Add(ErrorType.Buff, $"{this.DebugName} 유효하지 않은 buffId:{buffId}");
                continue;
            }

            this.buffTemplets.Add(buffTemplet);
        }
    }

    public void Validate()
    {
    }

    public void LoadGroupData(int groupId, JToken token)
    {
        this.GroupId = groupId;
    }

    public void Load(JToken token)
    {
        var buffId = token.GetString("ImmuneBuffId");
        if (this.buffIds.Add(buffId) == false)
        {
            ErrorMessage.Add(ErrorType.Buff, $"{this.DebugName} 버프 아이디 중복 설정. buffId:{buffId}");
        }
    }

    public override string ToString()
    {
        return string.Join(", ", this.buffTemplets.Select(e => e.Name));
    }
}
