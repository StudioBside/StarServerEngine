namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Strings;

public sealed class BattleTemplet : ITemplet, ISearchable
{
    private readonly List<NormalRewardGroupNRate> normalRewards = new();
    private readonly string nameId;
    private readonly string iconName;

    public BattleTemplet(JToken token)
    {
        this.Id = token.GetInt32("BattleID");
        this.nameId = token.GetString("BattleTitleName");
        this.iconName = token.GetString("m_IconName");
    }

    public static IEnumerable<BuffTemplet> Values => TempletContainer<BuffTemplet>.Values;
    int ITemplet.Key => this.Id;
    public int Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string DebugName => $"[{this.Id} {this.Name}]";
    public string DebugId => $"[{this.Id} {this.nameId}]";

    bool ISearchable.IsTarget(string keyword)
    {
        return this.Id.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               this.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    public void Join()
    {
        this.Name = StringTable.Instance.Find(this.nameId);
    }

    public void Validate()
    {
    }

    public sealed class NormalRewardGroupNRate
    {
        public int RewardGroupId { get; private set; }
        public int RewardRate { get; private set; }
        public NormalRewardGroupTemplet RewardGroupTemplet { get; private set; } = null!;

        public static NormalRewardGroupNRate Load(JToken token, int index)
        {
            return new NormalRewardGroupNRate
            {
                RewardGroupId = token.GetInt32("NormalRewardGroup"),
                RewardRate = token.GetInt32("NormalRewardRate"),
            };
        }

        public static NormalRewardGroupNRate Load(JToken token, string idKey, string rateKey)
        {
            return new NormalRewardGroupNRate
            {
                RewardGroupId = token.GetInt32(idKey),
                RewardRate = token.GetInt32(rateKey),
            };
        }

        public void Join()
        {
            var templet = NormalRewardGroupTemplet.Find(this.RewardGroupId);
            if (templet is null)
            {
                return;
            }

            this.RewardGroupTemplet = templet;
        }
    }
}
